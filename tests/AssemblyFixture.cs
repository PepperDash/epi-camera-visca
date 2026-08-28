using System.Reflection;
using System.Text.Json;

namespace PepperDash.Essentials.Plugins.Camera.Visca.Tests;

public static class AssemblyFixture
{
    private static readonly Lazy<MetadataLoadContext> LazyContext = new(CreateContext);
    private static readonly Lazy<Assembly> LazyAssembly = new(LoadPluginAssembly);

    private static string Configuration
    {
        get
        {
            // Derive build configuration from test output path: tests/bin/{Configuration}/net8.0/
            var baseDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            var parts = baseDir.Split(Path.DirectorySeparatorChar);
            return parts[^2]; // net8.0 is last, Configuration is second-to-last
        }
    }

    private static string PluginDllPath =>
        Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "src", "4Series", "bin", Configuration, "net8.0",
            "epi-camera-visca.4Series.dll"));

    private static string PluginOutputDir => Path.GetDirectoryName(PluginDllPath)!;

    public static MetadataLoadContext Context => LazyContext.Value;
    public static Assembly PluginAssembly => LazyAssembly.Value;

    private static string ProjectAssetsJsonPath =>
        Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "src", "obj", "project.assets.json"));

    private static MetadataLoadContext CreateContext()
    {
        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var dllByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Priority 1: Plugin output directory (has the correct versions)
        foreach (var dll in Directory.GetFiles(PluginOutputDir, "*.dll"))
            dllByName[Path.GetFileName(dll)] = dll;

        // Priority 2: .NET runtime directory
        foreach (var dll in Directory.GetFiles(runtimeDir, "*.dll"))
            dllByName.TryAdd(Path.GetFileName(dll), dll);

        // Priority 3: Resolve all dependencies from project.assets.json.
        // This includes compile-time-only references (e.g. PepperDash with ExcludeAssets=runtime)
        // that do not appear in the plugin's deps.json.
        foreach (var path in ResolveProjectAssetsAssemblies())
            dllByName.TryAdd(Path.GetFileName(path), path);

        return new MetadataLoadContext(new PathAssemblyResolver(dllByName.Values));
    }

    // TFM priority order for selecting the best lib/ subfolder from a NuGet package.
    private static readonly string[] TfmOrder =
        ["net8.0", "net7.0", "net6.0", "net5.0", "netstandard2.1", "netstandard2.0", "netstandard1.3"];

    private static IEnumerable<string> ResolveProjectAssetsAssemblies()
    {
        if (!File.Exists(ProjectAssetsJsonPath))
            yield break;

        using var stream = File.OpenRead(ProjectAssetsJsonPath);
        using var doc = JsonDocument.Parse(stream);

        // packageFolders lists the NuGet global packages directories (e.g. ~/.nuget/packages/)
        var nugetDirs = doc.RootElement.TryGetProperty("packageFolders", out var folders)
            ? folders.EnumerateObject().Select(f => f.Name).ToList()
            : [Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages")];

        if (!doc.RootElement.TryGetProperty("libraries", out var libraries))
            yield break;

        foreach (var lib in libraries.EnumerateObject())
        {
            if (!lib.Value.TryGetProperty("type", out var typeProp) || typeProp.GetString() != "package")
                continue;
            if (!lib.Value.TryGetProperty("path", out var pathProp))
                continue;
            if (!lib.Value.TryGetProperty("files", out var filesProp))
                continue;

            var packageRelPath = pathProp.GetString()!;

            // Collect the DLL paths available in this package, keyed by lib/{tfm}/ prefix
            var packageDlls = filesProp.EnumerateArray()
                .Select(f => f.GetString()!)
                .Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Pick the best TFM available in this package
            string? chosenTfm = null;
            foreach (var tfm in TfmOrder)
            {
                if (packageDlls.Any(f => f.StartsWith($"lib/{tfm}/", StringComparison.OrdinalIgnoreCase)))
                {
                    chosenTfm = tfm;
                    break;
                }
            }
            if (chosenTfm == null) continue;

            var tfmPrefix = $"lib/{chosenTfm}/";
            var tfmDlls = packageDlls.Where(f => f.StartsWith(tfmPrefix, StringComparison.OrdinalIgnoreCase));

            // Find the first NuGet package folder that actually contains this package
            foreach (var nugetDir in nugetDirs)
            {
                var packagePath = Path.Combine(nugetDir, packageRelPath);
                if (!Directory.Exists(packagePath)) continue;

                foreach (var dllFile in tfmDlls)
                {
                    var fullPath = Path.Combine(packagePath, dllFile.Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(fullPath))
                        yield return fullPath;
                }
                break;
            }
        }
    }

    private static Assembly LoadPluginAssembly()
    {
        if (!File.Exists(PluginDllPath))
            throw new FileNotFoundException(
                $"Plugin DLL not found at '{PluginDllPath}'. Build the plugin first with: dotnet build src/epi-camera-visca.4Series.csproj");
        return Context.LoadFromAssemblyPath(PluginDllPath);
    }

    public static List<Type> FindFactoryTypes(string baseTypePrefix = "EssentialsPluginDeviceFactory")
    {
        return PluginAssembly.GetTypes()
            .Where(t => !t.IsAbstract
                && t.BaseType is { IsGenericType: true }
                && t.BaseType.GetGenericTypeDefinition().Name.StartsWith(baseTypePrefix))
            .ToList();
    }

    public static string SourceDirectory =>
        Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "src"));
}
