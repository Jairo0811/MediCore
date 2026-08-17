using MediCore.Application.Common;
using MediCore.Application.Pharmacy;
using MediCore.Domain.Pharmacy;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Pharmacy;

public sealed class PharmacyService(MediCoreDbContext dbContext) : IPharmacyService
{
    public async Task<IReadOnlyCollection<DrugTypeResponse>> GetDrugTypesAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var query = dbContext.DrugTypes.AsNoTracking();
        if (!includeInactive) query = query.Where(item => item.IsActive);
        return await query.OrderBy(item => item.Name).Select(item => new DrugTypeResponse(item.Id, item.Name, item.Description, item.IsActive)).ToArrayAsync(cancellationToken);
    }

    public async Task<OperationResult<DrugTypeResponse>> CreateDrugTypeAsync(CreateDrugTypeRequest request, CancellationToken cancellationToken)
    {
        var name = Required(request.Name);
        if (name is null) return OperationResult<DrugTypeResponse>.Failure("name_required", "El nombre del tipo de fármaco es obligatorio.");
        if (await dbContext.DrugTypes.AnyAsync(item => item.Name == name, cancellationToken)) return OperationResult<DrugTypeResponse>.Failure("duplicate_name", "Ya existe un tipo de fármaco con ese nombre.");
        var entity = new DrugType(name, Optional(request.Description));
        dbContext.DrugTypes.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<DrugTypeResponse>.Success(Map(entity));
    }

    public async Task<OperationResult<DrugTypeResponse>> UpdateDrugTypeAsync(Guid id, UpdateDrugTypeRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.DrugTypes.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null) return OperationResult<DrugTypeResponse>.Failure("not_found", "Tipo de fármaco no encontrado.");
        var name = Required(request.Name);
        if (name is null) return OperationResult<DrugTypeResponse>.Failure("name_required", "El nombre del tipo de fármaco es obligatorio.");
        if (await dbContext.DrugTypes.AnyAsync(item => item.Id != id && item.Name == name, cancellationToken)) return OperationResult<DrugTypeResponse>.Failure("duplicate_name", "Ya existe un tipo de fármaco con ese nombre.");
        if (!request.IsActive && entity.IsActive && await dbContext.Medications.AnyAsync(item => item.DrugTypeId == id && item.IsActive, cancellationToken)) return OperationResult<DrugTypeResponse>.Failure("catalog_in_use", "No se puede desactivar un tipo de fármaco utilizado por medicamentos activos.");
        entity.Update(name, Optional(request.Description));
        if (request.IsActive) entity.Reactivate(); else entity.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<DrugTypeResponse>.Success(Map(entity));
    }

    public async Task<OperationResult<bool>> DeactivateDrugTypeAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.DrugTypes.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null) return OperationResult<bool>.Failure("not_found", "Tipo de fármaco no encontrado.");
        if (await dbContext.Medications.AnyAsync(item => item.DrugTypeId == id && item.IsActive, cancellationToken)) return OperationResult<bool>.Failure("catalog_in_use", "No se puede desactivar un tipo de fármaco utilizado por medicamentos activos.");
        entity.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<bool>.Success(true);
    }

    public async Task<IReadOnlyCollection<PharmaceuticalBrandResponse>> GetBrandsAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var query = dbContext.PharmaceuticalBrands.AsNoTracking();
        if (!includeInactive) query = query.Where(item => item.IsActive);
        return await query.OrderBy(item => item.Name).Select(item => new PharmaceuticalBrandResponse(item.Id, item.Name, item.ManufacturerCountry, item.Website, item.IsActive)).ToArrayAsync(cancellationToken);
    }

    public async Task<OperationResult<PharmaceuticalBrandResponse>> CreateBrandAsync(CreatePharmaceuticalBrandRequest request, CancellationToken cancellationToken)
    {
        var name = Required(request.Name);
        if (name is null) return OperationResult<PharmaceuticalBrandResponse>.Failure("name_required", "El nombre de la marca es obligatorio.");
        if (await dbContext.PharmaceuticalBrands.AnyAsync(item => item.Name == name, cancellationToken)) return OperationResult<PharmaceuticalBrandResponse>.Failure("duplicate_name", "Ya existe una marca farmacéutica con ese nombre.");
        var entity = new PharmaceuticalBrand(name, Optional(request.ManufacturerCountry), Optional(request.Website));
        dbContext.PharmaceuticalBrands.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<PharmaceuticalBrandResponse>.Success(Map(entity));
    }

    public async Task<OperationResult<PharmaceuticalBrandResponse>> UpdateBrandAsync(Guid id, UpdatePharmaceuticalBrandRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PharmaceuticalBrands.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null) return OperationResult<PharmaceuticalBrandResponse>.Failure("not_found", "Marca farmacéutica no encontrada.");
        var name = Required(request.Name);
        if (name is null) return OperationResult<PharmaceuticalBrandResponse>.Failure("name_required", "El nombre de la marca es obligatorio.");
        if (await dbContext.PharmaceuticalBrands.AnyAsync(item => item.Id != id && item.Name == name, cancellationToken)) return OperationResult<PharmaceuticalBrandResponse>.Failure("duplicate_name", "Ya existe una marca farmacéutica con ese nombre.");
        if (!request.IsActive && entity.IsActive && await dbContext.Medications.AnyAsync(item => item.PharmaceuticalBrandId == id && item.IsActive, cancellationToken)) return OperationResult<PharmaceuticalBrandResponse>.Failure("catalog_in_use", "No se puede desactivar una marca utilizada por medicamentos activos.");
        entity.Update(name, Optional(request.ManufacturerCountry), Optional(request.Website));
        if (request.IsActive) entity.Reactivate(); else entity.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<PharmaceuticalBrandResponse>.Success(Map(entity));
    }

    public async Task<OperationResult<bool>> DeactivateBrandAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PharmaceuticalBrands.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null) return OperationResult<bool>.Failure("not_found", "Marca farmacéutica no encontrada.");
        if (await dbContext.Medications.AnyAsync(item => item.PharmaceuticalBrandId == id && item.IsActive, cancellationToken)) return OperationResult<bool>.Failure("catalog_in_use", "No se puede desactivar una marca utilizada por medicamentos activos.");
        entity.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<bool>.Success(true);
    }

    public async Task<IReadOnlyCollection<StorageLocationResponse>> GetLocationsAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var query = dbContext.StorageLocations.AsNoTracking();
        if (!includeInactive) query = query.Where(item => item.IsActive);
        return await query.OrderBy(item => item.Code).Select(item => new StorageLocationResponse(item.Id, item.Code, item.Name, item.Description, item.IsActive)).ToArrayAsync(cancellationToken);
    }

    public async Task<OperationResult<StorageLocationResponse>> CreateLocationAsync(CreateStorageLocationRequest request, CancellationToken cancellationToken)
    {
        var code = Code(request.Code); var name = Required(request.Name);
        if (code is null || name is null) return OperationResult<StorageLocationResponse>.Failure("required_fields", "El código y el nombre de la ubicación son obligatorios.");
        if (await dbContext.StorageLocations.AnyAsync(item => item.Code == code, cancellationToken)) return OperationResult<StorageLocationResponse>.Failure("duplicate_code", "Ya existe una ubicación con ese código.");
        var entity = new StorageLocation(code, name, Optional(request.Description));
        dbContext.StorageLocations.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<StorageLocationResponse>.Success(Map(entity));
    }

    public async Task<OperationResult<StorageLocationResponse>> UpdateLocationAsync(Guid id, UpdateStorageLocationRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.StorageLocations.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null) return OperationResult<StorageLocationResponse>.Failure("not_found", "Ubicación no encontrada.");
        var code = Code(request.Code); var name = Required(request.Name);
        if (code is null || name is null) return OperationResult<StorageLocationResponse>.Failure("required_fields", "El código y el nombre de la ubicación son obligatorios.");
        if (await dbContext.StorageLocations.AnyAsync(item => item.Id != id && item.Code == code, cancellationToken)) return OperationResult<StorageLocationResponse>.Failure("duplicate_code", "Ya existe una ubicación con ese código.");
        if (!request.IsActive && entity.IsActive && await dbContext.Medications.AnyAsync(item => item.StorageLocationId == id && item.IsActive, cancellationToken)) return OperationResult<StorageLocationResponse>.Failure("catalog_in_use", "No se puede desactivar una ubicación utilizada por medicamentos activos.");
        entity.Update(code, name, Optional(request.Description));
        if (request.IsActive) entity.Reactivate(); else entity.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<StorageLocationResponse>.Success(Map(entity));
    }

    public async Task<OperationResult<bool>> DeactivateLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.StorageLocations.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null) return OperationResult<bool>.Failure("not_found", "Ubicación no encontrada.");
        if (await dbContext.Medications.AnyAsync(item => item.StorageLocationId == id && item.IsActive, cancellationToken)) return OperationResult<bool>.Failure("catalog_in_use", "No se puede desactivar una ubicación utilizada por medicamentos activos.");
        entity.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<bool>.Success(true);
    }

    public async Task<IReadOnlyCollection<MedicationResponse>> GetMedicationsAsync(string? search, Guid? drugTypeId, bool includeInactive, CancellationToken cancellationToken)
    {
        var query = dbContext.Medications.AsNoTracking();
        if (!includeInactive) query = query.Where(item => item.IsActive);
        if (drugTypeId.HasValue) query = query.Where(item => item.DrugTypeId == drugTypeId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(item => item.Code.Contains(term) || item.Name.Contains(term) || (item.GenericName != null && item.GenericName.Contains(term)) || (item.ActiveIngredient != null && item.ActiveIngredient.Contains(term)));
        }
        return await Project(query).OrderBy(item => item.Name).ToArrayAsync(cancellationToken);
    }

    public async Task<MedicationResponse?> GetMedicationByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await Project(dbContext.Medications.AsNoTracking().Where(item => item.Id == id)).SingleOrDefaultAsync(cancellationToken);

    public async Task<OperationResult<MedicationResponse>> CreateMedicationAsync(CreateMedicationRequest request, CancellationToken cancellationToken)
    {
        var validation = await ValidateMedicationAsync(request.Code, request.Name, request.DrugTypeId, request.PharmaceuticalBrandId, request.StorageLocationId, null, cancellationToken);
        if (!validation.Succeeded) return OperationResult<MedicationResponse>.Failure(validation.ErrorCode!, validation.ErrorMessage!);
        var entity = new Medication(Code(request.Code)!, request.Name.Trim(), Optional(request.GenericName), Optional(request.ActiveIngredient), Optional(request.Strength), Optional(request.DosageForm), Optional(request.UnitOfMeasure), request.DrugTypeId, request.PharmaceuticalBrandId, request.StorageLocationId, request.RequiresPrescription, request.IsControlledSubstance, Optional(request.Notes));
        dbContext.Medications.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<MedicationResponse>.Success((await GetMedicationByIdAsync(entity.Id, cancellationToken))!);
    }

    public async Task<OperationResult<MedicationResponse>> UpdateMedicationAsync(Guid id, UpdateMedicationRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Medications.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null) return OperationResult<MedicationResponse>.Failure("not_found", "Medicamento no encontrado.");
        var validation = await ValidateMedicationAsync(request.Code, request.Name, request.DrugTypeId, request.PharmaceuticalBrandId, request.StorageLocationId, id, cancellationToken);
        if (!validation.Succeeded) return OperationResult<MedicationResponse>.Failure(validation.ErrorCode!, validation.ErrorMessage!);
        entity.Update(Code(request.Code)!, request.Name.Trim(), Optional(request.GenericName), Optional(request.ActiveIngredient), Optional(request.Strength), Optional(request.DosageForm), Optional(request.UnitOfMeasure), request.DrugTypeId, request.PharmaceuticalBrandId, request.StorageLocationId, request.RequiresPrescription, request.IsControlledSubstance, Optional(request.Notes));
        if (request.IsActive) entity.Reactivate(); else entity.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<MedicationResponse>.Success((await GetMedicationByIdAsync(entity.Id, cancellationToken))!);
    }

    public async Task<OperationResult<bool>> DeactivateMedicationAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Medications.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null) return OperationResult<bool>.Failure("not_found", "Medicamento no encontrado.");
        entity.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<bool>.Success(true);
    }

    private async Task<OperationResult<bool>> ValidateMedicationAsync(string code, string name, Guid drugTypeId, Guid? brandId, Guid? locationId, Guid? currentId, CancellationToken cancellationToken)
    {
        var normalizedCode = Code(code);
        if (normalizedCode is null || Required(name) is null) return OperationResult<bool>.Failure("required_fields", "El código y el nombre del medicamento son obligatorios.");
        if (await dbContext.Medications.AnyAsync(item => item.Code == normalizedCode && (!currentId.HasValue || item.Id != currentId.Value), cancellationToken)) return OperationResult<bool>.Failure("duplicate_code", "Ya existe un medicamento con ese código.");
        if (!await dbContext.DrugTypes.AnyAsync(item => item.Id == drugTypeId && item.IsActive, cancellationToken)) return OperationResult<bool>.Failure("invalid_drug_type", "El tipo de fármaco no existe o está inactivo.");
        if (brandId.HasValue && !await dbContext.PharmaceuticalBrands.AnyAsync(item => item.Id == brandId.Value && item.IsActive, cancellationToken)) return OperationResult<bool>.Failure("invalid_brand", "La marca farmacéutica no existe o está inactiva.");
        if (locationId.HasValue && !await dbContext.StorageLocations.AnyAsync(item => item.Id == locationId.Value && item.IsActive, cancellationToken)) return OperationResult<bool>.Failure("invalid_location", "La ubicación no existe o está inactiva.");
        return OperationResult<bool>.Success(true);
    }

    private IQueryable<MedicationResponse> Project(IQueryable<Medication> query) => query.Select(medication => new MedicationResponse(
        medication.Id, medication.Code, medication.Name, medication.GenericName, medication.ActiveIngredient, medication.Strength, medication.DosageForm, medication.UnitOfMeasure,
        medication.DrugTypeId, dbContext.DrugTypes.Where(item => item.Id == medication.DrugTypeId).Select(item => item.Name).First(),
        medication.PharmaceuticalBrandId, dbContext.PharmaceuticalBrands.Where(item => item.Id == medication.PharmaceuticalBrandId).Select(item => item.Name).FirstOrDefault(),
        medication.StorageLocationId, dbContext.StorageLocations.Where(item => item.Id == medication.StorageLocationId).Select(item => item.Name).FirstOrDefault(),
        medication.RequiresPrescription, medication.IsControlledSubstance, medication.Notes, medication.IsActive, medication.CreatedAtUtc, medication.UpdatedAtUtc));

    private static DrugTypeResponse Map(DrugType entity) => new(entity.Id, entity.Name, entity.Description, entity.IsActive);
    private static PharmaceuticalBrandResponse Map(PharmaceuticalBrand entity) => new(entity.Id, entity.Name, entity.ManufacturerCountry, entity.Website, entity.IsActive);
    private static StorageLocationResponse Map(StorageLocation entity) => new(entity.Id, entity.Code, entity.Name, entity.Description, entity.IsActive);
    private static string? Required(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? Code(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
