using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Service;

namespace BuildingBlocks.SeriLog;
public sealed class EnvPackages
{
    private readonly Dictionary<string, string> _referencedAssemblies = new();
    private readonly string _serviceName;
    private readonly string _version;

    public EnvPackages()
    {
        var refAss = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
            .Where(r => !string.IsNullOrEmpty(r.Name) && r.Version != null);
        foreach (var assembly in refAss?.DistinctBy(r => r.Name))
            _referencedAssemblies.Add(assembly.Name, assembly.Version.ToString());

        _referencedAssemblies.ToJson();

        var entryAssembly = Assembly.GetEntryAssembly()?.GetName();
        _serviceName = entryAssembly?.Name?.Replace('.', '_')?.Replace('-', '_');
        _version = entryAssembly?.Version?.ToString();
    }

    public string AssemblyName() => _serviceName;

    public string AssemblyVersion() => _version;
}
