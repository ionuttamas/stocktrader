using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureBusinessServiceRole
{
    public class AzureLocalStorageTraceListener : XmlWriterTraceListener
    {
        public AzureLocalStorageTraceListener()
            : base(Path.Combine(GetLogDirectory().Path, "BslWcf.svclog"))
        {
        }

        private static DirectoryConfiguration GetLogDirectory()
        {
            DirectoryConfiguration directory = new DirectoryConfiguration
                                                   {
                                                       Container = "wad-tracefiles",
                                                       DirectoryQuotaInMB = 1000,
                                                       Path = RoleEnvironment.GetLocalResource("BslWcf.svclog").RootPath
                                                   };
            return directory;
        }
    }
}