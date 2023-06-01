using System.Reflection;

namespace ShevaEngine.Core
{
    public class VersionUtils
    {
        /// <summary>
        /// Get version.
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            return Assembly.GetEntryAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion!;
        }
    }
}
