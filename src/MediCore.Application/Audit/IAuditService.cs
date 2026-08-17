namespace MediCore.Application.Audit;

public interface IAuditService
{
    Task<IReadOnlyCollection<AuditLogResponse>> GetRecentAsync(int take, string? entityName, CancellationToken cancellationToken);
}
