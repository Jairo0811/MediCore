using MediCore.Domain.Common;

namespace MediCore.ArchitectureTests;

public sealed class DependencyRulesTests
{
    [Fact]
    public void Domain_does_not_reference_outer_layers()
    {
        var referencedAssemblies = typeof(BaseEntity)
            .Assembly
            .GetReferencedAssemblies()
            .Select(assembly => assembly.Name)
            .Where(name => name is not null)
            .ToArray();

        Assert.DoesNotContain("MediCore.Application", referencedAssemblies);
        Assert.DoesNotContain("MediCore.Infrastructure", referencedAssemblies);
        Assert.DoesNotContain("MediCore.Api", referencedAssemblies);
    }
}
