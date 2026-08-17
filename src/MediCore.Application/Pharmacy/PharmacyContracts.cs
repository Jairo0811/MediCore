namespace MediCore.Application.Pharmacy;

public sealed record DrugTypeResponse(Guid Id, string Name, string? Description, bool IsActive);
public sealed record CreateDrugTypeRequest(string Name, string? Description);
public sealed record UpdateDrugTypeRequest(string Name, string? Description, bool IsActive);

public sealed record PharmaceuticalBrandResponse(Guid Id, string Name, string? ManufacturerCountry, string? Website, bool IsActive);
public sealed record CreatePharmaceuticalBrandRequest(string Name, string? ManufacturerCountry, string? Website);
public sealed record UpdatePharmaceuticalBrandRequest(string Name, string? ManufacturerCountry, string? Website, bool IsActive);

public sealed record StorageLocationResponse(Guid Id, string Code, string Name, string? Description, bool IsActive);
public sealed record CreateStorageLocationRequest(string Code, string Name, string? Description);
public sealed record UpdateStorageLocationRequest(string Code, string Name, string? Description, bool IsActive);

public sealed record MedicationResponse(Guid Id, string Code, string Name, string? GenericName, string? ActiveIngredient, string? Strength, string? DosageForm, string? UnitOfMeasure, Guid DrugTypeId, string DrugTypeName, Guid? PharmaceuticalBrandId, string? PharmaceuticalBrandName, Guid? StorageLocationId, string? StorageLocationName, bool RequiresPrescription, bool IsControlledSubstance, string? Notes, bool IsActive, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
public sealed record CreateMedicationRequest(string Code, string Name, string? GenericName, string? ActiveIngredient, string? Strength, string? DosageForm, string? UnitOfMeasure, Guid DrugTypeId, Guid? PharmaceuticalBrandId, Guid? StorageLocationId, bool RequiresPrescription, bool IsControlledSubstance, string? Notes);
public sealed record UpdateMedicationRequest(string Code, string Name, string? GenericName, string? ActiveIngredient, string? Strength, string? DosageForm, string? UnitOfMeasure, Guid DrugTypeId, Guid? PharmaceuticalBrandId, Guid? StorageLocationId, bool RequiresPrescription, bool IsControlledSubstance, string? Notes, bool IsActive);
