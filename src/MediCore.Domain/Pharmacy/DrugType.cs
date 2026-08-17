using MediCore.Domain.Common;

namespace MediCore.Domain.Pharmacy;

public sealed class DrugType : BaseEntity
{
    private DrugType() { }
    public DrugType(string name, string? description) { Name = name; Description = description; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void Update(string name, string? description) { Name = name; Description = description; MarkAsUpdated(); }
    public void Deactivate() { IsActive = false; MarkAsUpdated(); }
    public void Reactivate() { IsActive = true; MarkAsUpdated(); }
}
