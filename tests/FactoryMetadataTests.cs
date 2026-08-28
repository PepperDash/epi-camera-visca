using System.Reflection;
using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugins.Camera.Visca.Tests;

public class FactoryMetadataTests
{
    [Fact]
    public void All_Factories_Inherit_MinimumEssentialsFrameworkVersion_Property()
    {
        var factories = AssemblyFixture.FindFactoryTypes();
        foreach (var factory in factories)
        {
            var prop = GetInheritedProperty(factory, "MinimumEssentialsFrameworkVersion");
            prop.Should().NotBeNull(
                $"factory '{factory.Name}' should inherit MinimumEssentialsFrameworkVersion from its base type");
        }
    }

    [Fact]
    public void All_Factories_Inherit_TypeNames_Property()
    {
        var factories = AssemblyFixture.FindFactoryTypes();
        foreach (var factory in factories)
        {
            var prop = GetInheritedProperty(factory, "TypeNames");
            prop.Should().NotBeNull(
                $"factory '{factory.Name}' should inherit TypeNames from its base type");
        }
    }

    [Theory]
    [InlineData("ViscaCameraFactory", "visca")]
    [InlineData("ViscaCameraFactory", "viscacamera")]
    public void Factory_Registers_Expected_TypeName(string factoryName, string expectedTypeName)
    {
        // Verify the factory constructor contains the expected type name string literal
        var type = AssemblyFixture.PluginAssembly.GetTypes()
            .First(t => t.Name == factoryName);
        var ctor = type.GetConstructor(Type.EmptyTypes);
        ctor.Should().NotBeNull($"factory '{factoryName}' should have a parameterless constructor");

        // Read the IL body to verify the string literal is present
        var body = ctor!.GetMethodBody();
        body.Should().NotBeNull();

        // Since MetadataLoadContext doesn't support IL reading, verify via assembly string resources
        // Use source verification as the factory is always rebuilt via ProjectReference
        var factoryFile = Path.Combine(AssemblyFixture.SourceDirectory, $"{factoryName}.cs");
        File.Exists(factoryFile).Should().BeTrue();
        var content = File.ReadAllText(factoryFile);
        content.Should().Contain($"\"{expectedTypeName}\"",
            $"factory '{factoryName}' should register type name '{expectedTypeName}'");
    }

    [Fact]
    public void No_Duplicate_TypeNames_Across_Factories()
    {
        // Collect all TypeNames string literals from factory source files
        var csFiles = Directory.GetFiles(AssemblyFixture.SourceDirectory, "*Factory.cs", SearchOption.AllDirectories);
        var allTypeNames = new List<string>();

        foreach (var file in csFiles)
        {
            var content = File.ReadAllText(file);
            if (!content.Contains("TypeNames")) continue;

            var matches = System.Text.RegularExpressions.Regex.Matches(
                content, @"TypeNames\s*=\s*new\s+List<string>\s*\{([^}]+)\}");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var names = System.Text.RegularExpressions.Regex.Matches(match.Groups[1].Value, @"""([^""]+)""")
                    .Cast<System.Text.RegularExpressions.Match>()
                    .Select(m => m.Groups[1].Value);
                allTypeNames.AddRange(names);
            }
        }

        allTypeNames.Should().OnlyHaveUniqueItems("TypeNames must not have duplicates across factories");
    }

    private static PropertyInfo? GetInheritedProperty(Type type, string propertyName)
    {
        var current = type;
        while (current != null)
        {
            var prop = current.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (prop != null) return prop;
            current = current.BaseType;
        }
        return null;
    }
}
