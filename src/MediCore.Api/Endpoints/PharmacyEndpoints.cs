using MediCore.Application.Common;
using MediCore.Application.Identity;
using MediCore.Application.Pharmacy;

namespace MediCore.Api.Endpoints;

public static class PharmacyEndpoints
{
    public static IEndpointRouteBuilder MapPharmacyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/pharmacy")
            .WithTags("Pharmacy")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Pharmacist, RoleNames.Doctor, RoleNames.Nurse, RoleNames.Auditor));

        group.MapGet("/drug-types", async (bool? includeInactive, IPharmacyService service, CancellationToken ct) => Results.Ok(await service.GetDrugTypesAsync(includeInactive ?? false, ct)));
        group.MapPost("/drug-types", async (CreateDrugTypeRequest request, IPharmacyService service, CancellationToken ct) => ToResult(await service.CreateDrugTypeAsync(request, ct), true)).RequirePharmacyWrite();
        group.MapPut("/drug-types/{id:guid}", async (Guid id, UpdateDrugTypeRequest request, IPharmacyService service, CancellationToken ct) => ToResult(await service.UpdateDrugTypeAsync(id, request, ct), false)).RequirePharmacyWrite();
        group.MapDelete("/drug-types/{id:guid}", async (Guid id, IPharmacyService service, CancellationToken ct) => ToResult(await service.DeactivateDrugTypeAsync(id, ct), false)).RequirePharmacyWrite();

        group.MapGet("/brands", async (bool? includeInactive, IPharmacyService service, CancellationToken ct) => Results.Ok(await service.GetBrandsAsync(includeInactive ?? false, ct)));
        group.MapPost("/brands", async (CreatePharmaceuticalBrandRequest request, IPharmacyService service, CancellationToken ct) => ToResult(await service.CreateBrandAsync(request, ct), true)).RequirePharmacyWrite();
        group.MapPut("/brands/{id:guid}", async (Guid id, UpdatePharmaceuticalBrandRequest request, IPharmacyService service, CancellationToken ct) => ToResult(await service.UpdateBrandAsync(id, request, ct), false)).RequirePharmacyWrite();
        group.MapDelete("/brands/{id:guid}", async (Guid id, IPharmacyService service, CancellationToken ct) => ToResult(await service.DeactivateBrandAsync(id, ct), false)).RequirePharmacyWrite();

        group.MapGet("/locations", async (bool? includeInactive, IPharmacyService service, CancellationToken ct) => Results.Ok(await service.GetLocationsAsync(includeInactive ?? false, ct)));
        group.MapPost("/locations", async (CreateStorageLocationRequest request, IPharmacyService service, CancellationToken ct) => ToResult(await service.CreateLocationAsync(request, ct), true)).RequirePharmacyWrite();
        group.MapPut("/locations/{id:guid}", async (Guid id, UpdateStorageLocationRequest request, IPharmacyService service, CancellationToken ct) => ToResult(await service.UpdateLocationAsync(id, request, ct), false)).RequirePharmacyWrite();
        group.MapDelete("/locations/{id:guid}", async (Guid id, IPharmacyService service, CancellationToken ct) => ToResult(await service.DeactivateLocationAsync(id, ct), false)).RequirePharmacyWrite();

        group.MapGet("/medications", async (string? search, Guid? drugTypeId, bool? includeInactive, IPharmacyService service, CancellationToken ct) => Results.Ok(await service.GetMedicationsAsync(search, drugTypeId, includeInactive ?? false, ct)));
        group.MapGet("/medications/{id:guid}", async (Guid id, IPharmacyService service, CancellationToken ct) =>
        {
            var medication = await service.GetMedicationByIdAsync(id, ct);
            return medication is null ? Results.NotFound() : Results.Ok(medication);
        });
        group.MapPost("/medications", async (CreateMedicationRequest request, IPharmacyService service, CancellationToken ct) => ToResult(await service.CreateMedicationAsync(request, ct), true)).RequirePharmacyWrite();
        group.MapPut("/medications/{id:guid}", async (Guid id, UpdateMedicationRequest request, IPharmacyService service, CancellationToken ct) => ToResult(await service.UpdateMedicationAsync(id, request, ct), false)).RequirePharmacyWrite();
        group.MapDelete("/medications/{id:guid}", async (Guid id, IPharmacyService service, CancellationToken ct) => ToResult(await service.DeactivateMedicationAsync(id, ct), false)).RequirePharmacyWrite();

        return endpoints;
    }

    private static RouteHandlerBuilder RequirePharmacyWrite(this RouteHandlerBuilder builder) => builder.RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Pharmacist));

    private static IResult ToResult<T>(OperationResult<T> result, bool created)
    {
        if (result.Succeeded) return created ? Results.Created(string.Empty, result.Value) : Results.Ok(result.Value);
        return result.ErrorCode switch
        {
            "not_found" => Results.NotFound(new { error = result.ErrorCode, message = result.ErrorMessage }),
            "duplicate_name" or "duplicate_code" or "catalog_in_use" => Results.Conflict(new { error = result.ErrorCode, message = result.ErrorMessage }),
            _ => Results.BadRequest(new { error = result.ErrorCode, message = result.ErrorMessage })
        };
    }
}
