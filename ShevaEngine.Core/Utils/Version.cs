using System;
using System.Reflection;

namespace ShevaEngine.Core
{
    public class Version
    {
        /// <summary>
        /// Get version.
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
#if WINDOWS_UAP                         
            Windows.ApplicationModel.PackageVersion version = Windows.ApplicationModel.Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Revision}";
#else
            return System.Reflection.Assembly.GetEntryAssembly()
                .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()
                .InformationalVersion;         
#endif
        }
    }
}
