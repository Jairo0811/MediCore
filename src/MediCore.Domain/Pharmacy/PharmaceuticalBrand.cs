using MediCore.Domain.Common;

namespace MediCore.Domain.Pharmacy;

public sealed class PharmaceuticalBrand : BaseEntity
{
    private PharmaceuticalBrand() { }
    public PharmaceuticalBrand(string name, string? manufacturerCountry, string? website) { Name = name; ManufacturerCountry = manufacturerCountry; Website = website; }
    public string Name { get; private set; } = string.Empty;
    public string? ManufacturerCountry { get; private set; }
    public string? Website { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void Update(string name, string? manufacturerCountry, string? website) { Name = name; ManufacturerCountry = manufacturerCountry; Website = website; MarkAsUpdated(); }
    public void Deactivate() { IsActive = false; MarkAsUpdated(); }
    public void Reactivate() { IsActive = true; MarkAsUpdated(); }
}
