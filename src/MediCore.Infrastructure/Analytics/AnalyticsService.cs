using MediCore.Application.Analytics;
using MediCore.Domain.Laboratory;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Analytics;

public sealed class AnalyticsService(MediCoreDbContext dbContext) : IAnalyticsService
{
    public async Task<DashboardSummaryResponse> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var todayStart = DateTime.UtcNow.Date; var tomorrow = todayStart.AddDays(1); var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30); var expiryLimit = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        return new(
            await dbContext.Patients.CountAsync(x => x.IsActive, cancellationToken),
            await dbContext.MedicalStaff.CountAsync(x => x.IsActive, cancellationToken),
            await dbContext.Appointments.CountAsync(x => x.ScheduledStartUtc >= todayStart && x.ScheduledStartUtc < tomorrow, cancellationToken),
            await dbContext.Consultations.CountAsync(x => (int)x.Status == 1, cancellationToken),
            await dbContext.Medications.CountAsync(x => x.IsActive, cancellationToken),
            await dbContext.InventoryLots.CountAsync(x => x.IsActive && x.QuantityOnHand <= x.ReorderPoint, cancellationToken),
            await dbContext.InventoryLots.CountAsync(x => x.IsActive && x.ExpirationDate <= expiryLimit, cancellationToken),
            await dbContext.LabOrders.CountAsync(x => x.Status == LabOrderStatus.Pending || x.Status == LabOrderStatus.InProgress, cancellationToken),
            await dbContext.LabOrders.CountAsync(x => x.Status == LabOrderStatus.Completed && x.UpdatedAtUtc >= thirtyDaysAgo, cancellationToken),
            await dbContext.Consultations.CountAsync(x => x.ConsultationDateUtc >= thirtyDaysAgo, cancellationToken));
    }

    public async Task<IReadOnlyCollection<InventoryAlertResponse>> GetInventoryAlertsAsync(CancellationToken cancellationToken)
    {
        var limit = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        var rows = await (from lot in dbContext.InventoryLots.AsNoTracking()
                          join med in dbContext.Medications.AsNoTracking() on lot.MedicationId equals med.Id
                          where lot.IsActive && (lot.QuantityOnHand <= lot.ReorderPoint || lot.ExpirationDate <= limit)
                          orderby lot.ExpirationDate
                          select new { lot, med }).ToArrayAsync(cancellationToken);
        return rows.Select(x => new InventoryAlertResponse(x.lot.Id, x.med.Code, x.med.Name, x.lot.LotNumber, x.lot.QuantityOnHand, x.lot.ReorderPoint, x.lot.ExpirationDate, x.lot.QuantityOnHand <= x.lot.ReorderPoint ? "LOW_STOCK" : "EXPIRING")).ToArray();
    }
}
