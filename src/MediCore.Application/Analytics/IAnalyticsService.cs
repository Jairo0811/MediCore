namespace MediCore.Application.Analytics;

public interface IAnalyticsService
{
    Task<DashboardSummaryResponse> GetDashboardAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<InventoryAlertResponse>> GetInventoryAlertsAsync(CancellationToken cancellationToken);
    Task<OperationalReportResponse> GetOperationalReportAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
}
