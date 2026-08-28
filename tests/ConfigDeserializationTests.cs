using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugins.Camera.Visca.Tests;

public class ConfigDeserializationTests
{
    [Theory]
    [InlineData("ViscaCameraConfig")]
    [InlineData("ViscaCameraPresetsConfig")]
    public void Config_Class_Exists(string className)
    {
        var type = AssemblyFixture.PluginAssembly.GetTypes()
            .FirstOrDefault(t => t.Name == className);
        type.Should().NotBeNull($"config class '{className}' should exist in the assembly");
    }

    [Theory]
    [InlineData("ViscaCameraConfig")]
    [InlineData("ViscaCameraPresetsConfig")]
    public void Config_Has_Parameterless_Constructor(string className)
    {
        var type = AssemblyFixture.PluginAssembly.GetTypes()
            .First(t => t.Name == className);
        var ctor = type.GetConstructor(Type.EmptyTypes);
        ctor.Should().NotBeNull($"config class '{className}' must have a parameterless constructor for JSON deserialization");
    }

    [Theory]
    [InlineData("ViscaCameraConfig", "control")]
    [InlineData("ViscaCameraConfig", "address")]
    [InlineData("ViscaCameraConfig", "panSpeed")]
    [InlineData("ViscaCameraConfig", "tiltSpeed")]
    [InlineData("ViscaCameraConfig", "ZoomSpeed")]
    [InlineData("ViscaCameraConfig", "FocusSpeed")]
    [InlineData("ViscaCameraConfig", "PrivacyOnPreset")]
    [InlineData("ViscaCameraConfig", "PrivacyOffPreset")]
    [InlineData("ViscaCameraConfig", "pollTimeMs")]
    [InlineData("ViscaCameraConfig", "presets")]
    public void Config_Property_Has_JsonPropertyAttribute(string className, string jsonName)
    {
        var type = AssemblyFixture.PluginAssembly.GetTypes()
            .First(t => t.Name == className);

        var properties = type.GetProperties();
        var hasAttribute = properties.Any(p =>
            p.CustomAttributes.Any(a =>
                a.AttributeType.Name == "JsonPropertyAttribute"
                && a.ConstructorArguments.Any(arg =>
                    string.Equals(arg.Value?.ToString(), jsonName, StringComparison.Ordinal))));

        hasAttribute.Should().BeTrue(
            $"config class '{className}' should have a property with [JsonProperty(\"{jsonName}\")] attribute in the compiled assembly");
    }
}
