namespace ClubAdmin.Shared.Abstractions;

/// <summary>
/// Contract for recording audit log entries.
/// </summary>
public interface IAuditLog
{
    Task RecordAsync(string entityType, string entityId, string action, string? details,
        CancellationToken cancellationToken = default);
}
