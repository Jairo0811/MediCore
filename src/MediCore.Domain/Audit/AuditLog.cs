using MediCore.Domain.Common;

namespace MediCore.Domain.Audit;

public sealed class AuditLog : BaseEntity
{
    private AuditLog()
    {
    }

    public AuditLog(
        Guid? userId,
        string action,
        string entityName,
        string? entityId,
        string? details,
        string? ipAddress)
    {
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        Details = details;
        IpAddress = ipAddress;
    }

    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string? Details { get; private set; }
    public string? IpAddress { get; private set; }
}
