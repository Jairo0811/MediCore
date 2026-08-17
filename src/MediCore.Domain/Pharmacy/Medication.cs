using MediCore.Domain.Common;

namespace MediCore.Domain.Pharmacy;

public sealed class Medication : BaseEntity
{
    private Medication() { }
    public Medication(string code, string name, string? genericName, string? activeIngredient, string? strength, string? dosageForm, string? unitOfMeasure, Guid drugTypeId, Guid? pharmaceuticalBrandId, Guid? storageLocationId, bool requiresPrescription, bool isControlledSubstance, string? notes)
    {
        Code = code; Name = name; GenericName = genericName; ActiveIngredient = activeIngredient; Strength = strength; DosageForm = dosageForm; UnitOfMeasure = unitOfMeasure; DrugTypeId = drugTypeId; PharmaceuticalBrandId = pharmaceuticalBrandId; StorageLocationId = storageLocationId; RequiresPrescription = requiresPrescription; IsControlledSubstance = isControlledSubstance; Notes = notes;
    }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? GenericName { get; private set; }
    public string? ActiveIngredient { get; private set; }
    public string? Strength { get; private set; }
    public string? DosageForm { get; private set; }
    public string? UnitOfMeasure { get; private set; }
    public Guid DrugTypeId { get; private set; }
    public Guid? PharmaceuticalBrandId { get; private set; }
    public Guid? StorageLocationId { get; private set; }
    public bool RequiresPrescription { get; private set; }
    public bool IsControlledSubstance { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void Update(string code, string name, string? genericName, string? activeIngredient, string? strength, string? dosageForm, string? unitOfMeasure, Guid drugTypeId, Guid? pharmaceuticalBrandId, Guid? storageLocationId, bool requiresPrescription, bool isControlledSubstance, string? notes)
    {
        Code = code; Name = name; GenericName = genericName; ActiveIngredient = activeIngredient; Strength = strength; DosageForm = dosageForm; UnitOfMeasure = unitOfMeasure; DrugTypeId = drugTypeId; PharmaceuticalBrandId = pharmaceuticalBrandId; StorageLocationId = storageLocationId; RequiresPrescription = requiresPrescription; IsControlledSubstance = isControlledSubstance; Notes = notes; MarkAsUpdated();
    }
    public void Deactivate() { IsActive = false; MarkAsUpdated(); }
    public void Reactivate() { IsActive = true; MarkAsUpdated(); }
}
