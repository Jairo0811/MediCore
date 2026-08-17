using MediCore.Domain.Common;

namespace MediCore.Domain.Laboratory;

public enum LabOrderStatus { Pending = 1, InProgress = 2, Completed = 3, Cancelled = 4 }
public enum LabResultStatus { Pending = 1, Completed = 2 }

public sealed class LabTestDefinition : BaseEntity
{
    private LabTestDefinition() { }
    public LabTestDefinition(string code, string name, string? sampleType, string? unit, string? referenceRange)
    { Code = code; Name = name; SampleType = sampleType; Unit = unit; ReferenceRange = referenceRange; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? SampleType { get; private set; }
    public string? Unit { get; private set; }
    public string? ReferenceRange { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void Deactivate() { IsActive = false; MarkAsUpdated(); }
}

public sealed class LabOrder : BaseEntity
{
    private LabOrder() { }
    public LabOrder(Guid patientId, Guid requestedByMedicalStaffId, Guid? consultationId, string? clinicalNotes)
    { PatientId = patientId; RequestedByMedicalStaffId = requestedByMedicalStaffId; ConsultationId = consultationId; ClinicalNotes = clinicalNotes; OrderedAtUtc = DateTime.UtcNow; }
    public Guid PatientId { get; private set; }
    public Guid RequestedByMedicalStaffId { get; private set; }
    public Guid? ConsultationId { get; private set; }
    public string? ClinicalNotes { get; private set; }
    public DateTime OrderedAtUtc { get; private set; }
    public LabOrderStatus Status { get; private set; } = LabOrderStatus.Pending;
    public void MarkInProgress() { if (Status == LabOrderStatus.Pending) Status = LabOrderStatus.InProgress; MarkAsUpdated(); }
    public void Complete() { Status = LabOrderStatus.Completed; MarkAsUpdated(); }
}

public sealed class LabOrderItem : BaseEntity
{
    private LabOrderItem() { }
    public LabOrderItem(Guid labOrderId, Guid labTestDefinitionId) { LabOrderId = labOrderId; LabTestDefinitionId = labTestDefinitionId; }
    public Guid LabOrderId { get; private set; }
    public Guid LabTestDefinitionId { get; private set; }
    public string? ResultValue { get; private set; }
    public string? ResultText { get; private set; }
    public string? ResultedBy { get; private set; }
    public DateTime? ResultedAtUtc { get; private set; }
    public LabResultStatus Status { get; private set; } = LabResultStatus.Pending;
    public void SetResult(string? resultValue, string? resultText, string resultedBy)
    { ResultValue = resultValue; ResultText = resultText; ResultedBy = resultedBy; ResultedAtUtc = DateTime.UtcNow; Status = LabResultStatus.Completed; MarkAsUpdated(); }
}
