using MediCore.Domain.Laboratory;

namespace MediCore.Application.Laboratory;

public sealed record CreateLabTestDefinitionRequest(string Code, string Name, string? SampleType, string? Unit, string? ReferenceRange);
public sealed record CreateLabOrderRequest(Guid PatientId, Guid RequestedByMedicalStaffId, Guid? ConsultationId, string? ClinicalNotes, Guid[] TestDefinitionIds);
public sealed record RecordLabResultRequest(string? ResultValue, string? ResultText);
public sealed record LabTestDefinitionResponse(Guid Id, string Code, string Name, string? SampleType, string? Unit, string? ReferenceRange, bool IsActive);
public sealed record LabOrderItemResponse(Guid Id, Guid LabTestDefinitionId, string TestCode, string TestName, string? Unit, string? ReferenceRange, string? ResultValue, string? ResultText, string? ResultedBy, DateTime? ResultedAtUtc, LabResultStatus Status);
public sealed record LabOrderResponse(Guid Id, Guid PatientId, string PatientName, Guid RequestedByMedicalStaffId, string RequestedByName, Guid? ConsultationId, string? ClinicalNotes, DateTime OrderedAtUtc, LabOrderStatus Status, IReadOnlyCollection<LabOrderItemResponse> Items);
