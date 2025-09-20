using System.Linq;
using System.Reflection;

namespace ShevaEngine.Core;

public class VersionUtils
{
    public static string GetVersion()
    {
        if (Assembly.GetEntryAssembly() is Assembly entryAssembly &&
            entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>() is AssemblyInformationalVersionAttribute versionAttribute &&
            versionAttribute.InformationalVersion is string version &&
            version.Split('+').FirstOrDefault() is string versionShort)
        {
            return versionShort;
        }

        return "Unknown";
    }
}
