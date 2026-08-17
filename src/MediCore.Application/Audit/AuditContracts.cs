namespace MediCore.Application.Audit;

public sealed record AuditLogResponse(Guid Id, Guid? UserId, string Action, string EntityName, string? EntityId, string? Details, string? IpAddress, DateTime CreatedAtUtc);
