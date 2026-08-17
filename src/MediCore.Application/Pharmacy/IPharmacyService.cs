using MediCore.Application.Common;

namespace MediCore.Application.Pharmacy;

public interface IPharmacyService
{
    Task<IReadOnlyCollection<DrugTypeResponse>> GetDrugTypesAsync(bool includeInactive, CancellationToken cancellationToken);
    Task<OperationResult<DrugTypeResponse>> CreateDrugTypeAsync(CreateDrugTypeRequest request, CancellationToken cancellationToken);
    Task<OperationResult<DrugTypeResponse>> UpdateDrugTypeAsync(Guid id, UpdateDrugTypeRequest request, CancellationToken cancellationToken);
    Task<OperationResult<bool>> DeactivateDrugTypeAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PharmaceuticalBrandResponse>> GetBrandsAsync(bool includeInactive, CancellationToken cancellationToken);
    Task<OperationResult<PharmaceuticalBrandResponse>> CreateBrandAsync(CreatePharmaceuticalBrandRequest request, CancellationToken cancellationToken);
    Task<OperationResult<PharmaceuticalBrandResponse>> UpdateBrandAsync(Guid id, UpdatePharmaceuticalBrandRequest request, CancellationToken cancellationToken);
    Task<OperationResult<bool>> DeactivateBrandAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StorageLocationResponse>> GetLocationsAsync(bool includeInactive, CancellationToken cancellationToken);
    Task<OperationResult<StorageLocationResponse>> CreateLocationAsync(CreateStorageLocationRequest request, CancellationToken cancellationToken);
    Task<OperationResult<StorageLocationResponse>> UpdateLocationAsync(Guid id, UpdateStorageLocationRequest request, CancellationToken cancellationToken);
    Task<OperationResult<bool>> DeactivateLocationAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MedicationResponse>> GetMedicationsAsync(string? search, Guid? drugTypeId, bool includeInactive, CancellationToken cancellationToken);
    Task<MedicationResponse?> GetMedicationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult<MedicationResponse>> CreateMedicationAsync(CreateMedicationRequest request, CancellationToken cancellationToken);
    Task<OperationResult<MedicationResponse>> UpdateMedicationAsync(Guid id, UpdateMedicationRequest request, CancellationToken cancellationToken);
    Task<OperationResult<bool>> DeactivateMedicationAsync(Guid id, CancellationToken cancellationToken);
}
