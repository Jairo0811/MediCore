using MediCore.Application.Audit;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Audit;

public sealed class AuditService(MediCoreDbContext dbContext) : IAuditService
{
    public async Task<IReadOnlyCollection<AuditLogResponse>> GetRecentAsync(int take, string? entityName, CancellationToken cancellationToken)
    {
        take = Math.Clamp(take, 1, 500);
        var query = dbContext.AuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(entityName)) query = query.Where(x => x.EntityName == entityName.Trim());
        return await query.OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .Select(x => new AuditLogResponse(x.Id, x.UserId, x.Action, x.EntityName, x.EntityId, x.Details, x.IpAddress, x.CreatedAtUtc))
            .ToArrayAsync(cancellationToken);
    }
}
