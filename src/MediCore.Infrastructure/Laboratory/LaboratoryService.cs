using MediCore.Application.Common;
using MediCore.Application.Laboratory;
using MediCore.Domain.Laboratory;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Laboratory;

public sealed class LaboratoryService(MediCoreDbContext dbContext) : ILaboratoryService
{
    public async Task<IReadOnlyCollection<LabTestDefinitionResponse>> GetDefinitionsAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var query = dbContext.LabTestDefinitions.AsNoTracking();
        if (!includeInactive) query = query.Where(x => x.IsActive);
        return await query.OrderBy(x => x.Name).Select(x => new LabTestDefinitionResponse(x.Id, x.Code, x.Name, x.SampleType, x.Unit, x.ReferenceRange, x.IsActive)).ToArrayAsync(cancellationToken);
    }

    public async Task<OperationResult<LabTestDefinitionResponse>> CreateDefinitionAsync(CreateLabTestDefinitionRequest request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant(); var name = request.Name.Trim();
        if (code.Length == 0 || name.Length == 0) return OperationResult<LabTestDefinitionResponse>.Failure("required", "Código y nombre son obligatorios.");
        if (await dbContext.LabTestDefinitions.AnyAsync(x => x.Code == code, cancellationToken)) return OperationResult<LabTestDefinitionResponse>.Failure("code_in_use", "El código de prueba ya existe.");
        var entity = new LabTestDefinition(code, name, N(request.SampleType), N(request.Unit), N(request.ReferenceRange));
        dbContext.LabTestDefinitions.Add(entity); await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<LabTestDefinitionResponse>.Success(new(entity.Id, entity.Code, entity.Name, entity.SampleType, entity.Unit, entity.ReferenceRange, entity.IsActive));
    }

    public async Task<IReadOnlyCollection<LabOrderResponse>> GetOrdersAsync(Guid? patientId, CancellationToken cancellationToken)
    {
        var query = dbContext.LabOrders.AsNoTracking();
        if (patientId.HasValue) query = query.Where(x => x.PatientId == patientId.Value);
        var orders = await query.OrderByDescending(x => x.OrderedAtUtc).ToArrayAsync(cancellationToken);
        var result = new List<LabOrderResponse>(orders.Length);
        foreach (var order in orders) result.Add(await MapOrderAsync(order, cancellationToken));
        return result;
    }

    public async Task<OperationResult<LabOrderResponse>> CreateOrderAsync(CreateLabOrderRequest request, CancellationToken cancellationToken)
    {
        if (request.TestDefinitionIds is null || request.TestDefinitionIds.Length == 0) return OperationResult<LabOrderResponse>.Failure("tests_required", "La orden debe contener al menos una prueba.");
        if (!await dbContext.Patients.AnyAsync(x => x.Id == request.PatientId && x.IsActive, cancellationToken) || !await dbContext.MedicalStaff.AnyAsync(x => x.Id == request.RequestedByMedicalStaffId && x.IsActive, cancellationToken))
            return OperationResult<LabOrderResponse>.Failure("invalid_reference", "Paciente y personal solicitante deben existir y estar activos.");
        var distinctIds = request.TestDefinitionIds.Distinct().ToArray();
        var activeCount = await dbContext.LabTestDefinitions.CountAsync(x => distinctIds.Contains(x.Id) && x.IsActive, cancellationToken);
        if (activeCount != distinctIds.Length) return OperationResult<LabOrderResponse>.Failure("invalid_test", "Una o más pruebas no existen o están inactivas.");

        var order = new LabOrder(request.PatientId, request.RequestedByMedicalStaffId, request.ConsultationId, N(request.ClinicalNotes));
        dbContext.LabOrders.Add(order);
        foreach (var id in distinctIds) dbContext.LabOrderItems.Add(new LabOrderItem(order.Id, id));
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<LabOrderResponse>.Success(await MapOrderAsync(order, cancellationToken));
    }

    public async Task<OperationResult<LabOrderResponse>> RecordResultAsync(Guid itemId, RecordLabResultRequest request, string resultedBy, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ResultValue) && string.IsNullOrWhiteSpace(request.ResultText)) return OperationResult<LabOrderResponse>.Failure("result_required", "Debe registrar un valor o comentario de resultado.");
        var item = await dbContext.LabOrderItems.SingleOrDefaultAsync(x => x.Id == itemId, cancellationToken);
        if (item is null) return OperationResult<LabOrderResponse>.Failure("not_found", "Detalle de laboratorio no encontrado.");
        var order = await dbContext.LabOrders.SingleAsync(x => x.Id == item.LabOrderId, cancellationToken);
        order.MarkInProgress(); item.SetResult(N(request.ResultValue), N(request.ResultText), resultedBy);
        await dbContext.SaveChangesAsync(cancellationToken);
        var pending = await dbContext.LabOrderItems.AnyAsync(x => x.LabOrderId == order.Id && x.Status != LabResultStatus.Completed, cancellationToken);
        if (!pending) { order.Complete(); await dbContext.SaveChangesAsync(cancellationToken); }
        return OperationResult<LabOrderResponse>.Success(await MapOrderAsync(order, cancellationToken));
    }

    private async Task<LabOrderResponse> MapOrderAsync(LabOrder order, CancellationToken ct)
    {
        var patient = await dbContext.Patients.AsNoTracking().SingleAsync(x => x.Id == order.PatientId, ct);
        var staff = await dbContext.MedicalStaff.AsNoTracking().SingleAsync(x => x.Id == order.RequestedByMedicalStaffId, ct);
        var items = await (from item in dbContext.LabOrderItems.AsNoTracking()
                           join test in dbContext.LabTestDefinitions.AsNoTracking() on item.LabTestDefinitionId equals test.Id
                           where item.LabOrderId == order.Id
                           orderby test.Name
                           select new LabOrderItemResponse(item.Id, test.Id, test.Code, test.Name, test.Unit, test.ReferenceRange, item.ResultValue, item.ResultText, item.ResultedBy, item.ResultedAtUtc, item.Status)).ToArrayAsync(ct);
        return new(order.Id, patient.Id, patient.FullName, staff.Id, staff.FullName, order.ConsultationId, order.ClinicalNotes, order.OrderedAtUtc, order.Status, items);
    }
    private static string? N(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
