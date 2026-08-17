using MediCore.Domain.Pharmacy;

namespace MediCore.UnitTests.Pharmacy;

public sealed class MedicationTests
{
    [Fact]
    public void Medication_CanBeUpdatedAndReactivated()
    {
        var drugTypeId = Guid.NewGuid();
        var medication = new Medication("MED-001", "Acetaminofén", "Paracetamol", "Paracetamol", "500 mg", "Tableta", "unidad", drugTypeId, null, null, false, false, null);
        medication.Deactivate();
        medication.Update("MED-001", "Acetaminofén 500 mg", "Paracetamol", "Paracetamol", "500 mg", "Tableta", "unidad", drugTypeId, null, null, false, false, "Uso oral");
        medication.Reactivate();
        Assert.True(medication.IsActive);
        Assert.Equal("Acetaminofén 500 mg", medication.Name);
        Assert.Equal("Uso oral", medication.Notes);
        Assert.NotNull(medication.UpdatedAtUtc);
    }
}
