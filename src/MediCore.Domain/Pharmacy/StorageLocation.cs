using MediCore.Domain.Common;

namespace MediCore.Domain.Pharmacy;

public sealed class StorageLocation : BaseEntity
{
    private StorageLocation() { }
    public StorageLocation(string code, string name, string? description) { Code = code; Name = name; Description = description; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void Update(string code, string name, string? description) { Code = code; Name = name; Description = description; MarkAsUpdated(); }
    public void Deactivate() { IsActive = false; MarkAsUpdated(); }
    public void Reactivate() { IsActive = true; MarkAsUpdated(); }
}
