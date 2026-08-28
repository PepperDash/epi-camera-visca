using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugins.Camera.Visca.Tests;

public class FactoryDiscoveryTests
{
    [Fact]
    public void Assembly_Loads_Successfully()
    {
        var assembly = AssemblyFixture.PluginAssembly;
        assembly.Should().NotBeNull();
    }

    [Fact]
    public void Assembly_Name_Matches_Expected()
    {
        var assembly = AssemblyFixture.PluginAssembly;
        assembly.GetName().Name.Should().Be("epi-camera-visca.4Series");
    }

    [Fact]
    public void Factory_Count_Matches_Expected()
    {
        var factories = AssemblyFixture.FindFactoryTypes();
        factories.Should().HaveCount(1, "there should be exactly 1 factory (ViscaCameraFactory)");
    }

    [Theory]
    [InlineData("ViscaCameraFactory")]
    public void Factory_Exists_ByName(string factoryName)
    {
        var type = AssemblyFixture.PluginAssembly.GetTypes()
            .FirstOrDefault(t => t.Name == factoryName);
        type.Should().NotBeNull($"factory '{factoryName}' should exist in the assembly");
    }

    [Fact]
    public void All_Factories_Have_Parameterless_Constructor()
    {
        var factories = AssemblyFixture.FindFactoryTypes();
        foreach (var factory in factories)
        {
            var ctor = factory.GetConstructor(Type.EmptyTypes);
            ctor.Should().NotBeNull($"factory '{factory.Name}' must have a parameterless constructor for plugin discovery");
        }
    }
}
