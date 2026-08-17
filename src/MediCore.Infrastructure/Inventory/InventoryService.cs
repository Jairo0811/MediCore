using MediCore.Application.Common;
using MediCore.Application.Inventory;
using MediCore.Domain.Inventory;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Inventory;

public sealed class InventoryService(MediCoreDbContext dbContext) : IInventoryService
{
    public async Task<IReadOnlyCollection<InventoryLotResponse>> GetLotsAsync(string? search, bool lowStockOnly, int? expiringWithinDays, CancellationToken cancellationToken)
    {
        var query = from lot in dbContext.InventoryLots.AsNoTracking()
                    join medication in dbContext.Medications.AsNoTracking() on lot.MedicationId equals medication.Id
                    join location in dbContext.StorageLocations.AsNoTracking() on lot.StorageLocationId equals location.Id
                    where lot.IsActive
                    select new { lot, medication, location };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.medication.Name.Contains(term) || x.medication.Code.Contains(term) || x.lot.LotNumber.Contains(term));
        }

        if (lowStockOnly) query = query.Where(x => x.lot.QuantityOnHand <= x.lot.ReorderPoint);
        if (expiringWithinDays.HasValue)
        {
            var limit = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(Math.Max(0, expiringWithinDays.Value)));
            query = query.Where(x => x.lot.ExpirationDate <= limit);
        }

        var rows = await query.OrderBy(x => x.lot.ExpirationDate).ThenBy(x => x.medication.Name).ToArrayAsync(cancellationToken);
        return rows.Select(x => Map(x.lot, x.medication.Code, x.medication.Name, x.location.Name)).ToArray();
    }

    public async Task<IReadOnlyCollection<InventoryMovementResponse>> GetKardexAsync(Guid lotId, CancellationToken cancellationToken) =>
        await dbContext.InventoryMovements.AsNoTracking()
            .Where(x => x.InventoryLotId == lotId)
            .OrderByDescending(x => x.OccurredAtUtc)
            .Select(x => new InventoryMovementResponse(x.Id, x.InventoryLotId, x.Type, x.QuantityDelta, x.BalanceAfter, x.PerformedBy, x.Reference, x.Notes, x.OccurredAtUtc))
            .ToArrayAsync(cancellationToken);

    public async Task<OperationResult<InventoryLotResponse>> CreateLotAsync(CreateInventoryLotRequest request, string performedBy, CancellationToken cancellationToken)
    {
        if (request.InitialQuantity < 0 || request.ReorderPoint < 0 || request.UnitCost < 0)
            return OperationResult<InventoryLotResponse>.Failure("invalid_values", "Cantidad, punto de reposición y costo deben ser valores no negativos.");
        if (string.IsNullOrWhiteSpace(request.LotNumber))
            return OperationResult<InventoryLotResponse>.Failure("lot_required", "El número de lote es obligatorio.");

        var medication = await dbContext.Medications.SingleOrDefaultAsync(x => x.Id == request.MedicationId && x.IsActive, cancellationToken);
        var location = await dbContext.StorageLocations.SingleOrDefaultAsync(x => x.Id == request.StorageLocationId && x.IsActive, cancellationToken);
        if (medication is null || location is null)
            return OperationResult<InventoryLotResponse>.Failure("invalid_reference", "El medicamento y la ubicación deben existir y estar activos.");

        var lotNumber = request.LotNumber.Trim();
        if (await dbContext.InventoryLots.AnyAsync(x => x.MedicationId == request.MedicationId && x.LotNumber == lotNumber, cancellationToken))
            return OperationResult<InventoryLotResponse>.Failure("lot_in_use", "Ya existe ese lote para el medicamento seleccionado.");

        var lot = new InventoryLot(request.MedicationId, request.StorageLocationId, lotNumber, request.ExpirationDate, request.UnitCost, request.InitialQuantity, request.ReorderPoint);
        dbContext.InventoryLots.Add(lot);
        if (request.InitialQuantity > 0)
            dbContext.InventoryMovements.Add(new InventoryMovement(lot.Id, InventoryMovementType.Receipt, request.InitialQuantity, request.InitialQuantity, performedBy, "Apertura de lote", "Existencia inicial"));
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<InventoryLotResponse>.Success(Map(lot, medication.Code, medication.Name, location.Name));
    }

    public async Task<OperationResult<InventoryLotResponse>> RecordMovementAsync(Guid lotId, CreateInventoryMovementRequest request, string performedBy, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0) return OperationResult<InventoryLotResponse>.Failure("quantity_required", "La cantidad debe ser mayor que cero.");
        var lot = await dbContext.InventoryLots.SingleOrDefaultAsync(x => x.Id == lotId && x.IsActive, cancellationToken);
        if (lot is null) return OperationResult<InventoryLotResponse>.Failure("not_found", "Lote no encontrado.");

        var delta = request.Type switch
        {
            InventoryMovementType.Receipt or InventoryMovementType.AdjustmentIncrease => request.Quantity,
            InventoryMovementType.Dispense or InventoryMovementType.AdjustmentDecrease => -request.Quantity,
            _ => 0
        };
        if (delta == 0) return OperationResult<InventoryLotResponse>.Failure("invalid_type", "Tipo de movimiento no válido.");
        if (lot.QuantityOnHand + delta < 0) return OperationResult<InventoryLotResponse>.Failure("insufficient_stock", "Existencia insuficiente para completar el movimiento.");

        lot.ApplyMovement(delta);
        dbContext.InventoryMovements.Add(new InventoryMovement(lot.Id, request.Type, delta, lot.QuantityOnHand, performedBy, Normalize(request.Reference), Normalize(request.Notes)));
        await dbContext.SaveChangesAsync(cancellationToken);

        var medication = await dbContext.Medications.AsNoTracking().SingleAsync(x => x.Id == lot.MedicationId, cancellationToken);
        var location = await dbContext.StorageLocations.AsNoTracking().SingleAsync(x => x.Id == lot.StorageLocationId, cancellationToken);
        return OperationResult<InventoryLotResponse>.Success(Map(lot, medication.Code, medication.Name, location.Name));
    }

    private static InventoryLotResponse Map(InventoryLot lot, string medicationCode, string medicationName, string locationName) =>
        new(lot.Id, lot.MedicationId, medicationCode, medicationName, lot.StorageLocationId, locationName, lot.LotNumber, lot.ExpirationDate, lot.UnitCost, lot.QuantityOnHand, lot.ReorderPoint, lot.IsLowStock, lot.IsActive);

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
