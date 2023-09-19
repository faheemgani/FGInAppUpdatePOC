using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGInAppUpdatePOC
{
    public class ApplicationInfo
    {
        public string appCurrentVersion { get; set; }
        public bool updatesAvailable { get; set; } = false;
        public string appInstallerUri { get; set; }
        public string appInstallerVersion { get; set; }
        public string appInstallerMainPackageName { get; set; }
        public string appInstallerMainPackageVersion { get; set; } = "0.0.0.0";
        public string appInstallerMainPackageUri { get; set; }
        public string appInstallerEnvironment { get; set; } = "N/A";
    }
}
