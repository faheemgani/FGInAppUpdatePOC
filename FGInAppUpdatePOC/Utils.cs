using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Management.Deployment;

namespace FGInAppUpdatePOC
{
    public static class Utils
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task TryToUpdateAsync(CancellationToken token, ApplicationInfo applicationInfo)
        {
            if (!applicationInfo.updatesAvailable)
            {
                throw new ArgumentException("The app is already up-to-date.");
            }

            PackageManager packageManager = new PackageManager();

            DeploymentOptions deploymentOptions = DeploymentOptions.ForceApplicationShutdown | DeploymentOptions.ForceUpdateFromAnyVersion;

            token.ThrowIfCancellationRequested();

            var update = await packageManager.UpdatePackageAsync
                          (new System.Uri(applicationInfo.appInstallerMainPackageUri),
                           null,
                           deploymentOptions);

        }


        public static bool UpdatesAvailable(ApplicationInfo applicationInfo)
        {
            bool updatesAvailable = false;

            if (applicationInfo.appCurrentVersion != applicationInfo.appInstallerMainPackageVersion)
                updatesAvailable = true;

            return updatesAvailable;
        }

        public static async Task<ApplicationInfo> CollectApplicationInfo(string appInstallerUri)
        {
            ApplicationInfo applicationInfo = new ApplicationInfo();

            try
            {
                var content = await _client.GetStringAsync(new System.Uri(appInstallerUri));

                var document = new XmlDocument();
                document.LoadXml(content);

                var root = document.DocumentElement;
                applicationInfo.appInstallerUri = root.GetAttribute("Uri");
                applicationInfo.appInstallerVersion = root.GetAttribute("Version");

                var mainPackage = root["MainPackage"];
                applicationInfo.appInstallerMainPackageName = mainPackage.GetAttribute("Name");
                applicationInfo.appInstallerMainPackageVersion = mainPackage.GetAttribute("Version");
                applicationInfo.appInstallerMainPackageUri = mainPackage.GetAttribute("Uri");


                applicationInfo.appInstallerUri = appInstallerUri;
                applicationInfo.appInstallerEnvironment = GetAppInstallerEnvironment(appInstallerUri);

                applicationInfo.appCurrentVersion = GetCurrentAppVersion();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
            return applicationInfo;

        }

        public static string GetAppInstallerEnvironment(string appInstallerUri)
        {
            string environment = "";

            int lastIndex = appInstallerUri.LastIndexOf('/');

            int secondLastIndex = appInstallerUri.LastIndexOf('/', lastIndex - 1);

            if (secondLastIndex != -1 && secondLastIndex < lastIndex)
            {
                environment = appInstallerUri.Substring(secondLastIndex + 1, lastIndex - secondLastIndex - 1);
            }
            else
            {
                environment = "N/A";
            }
            return environment;
        }

        public static string GetCurrentAppVersion()
        {
            var currentPackage = Windows.ApplicationModel.Package.Current;
            
            return $"{currentPackage.Id.Version.Major}.{currentPackage.Id.Version.Minor}.{currentPackage.Id.Version.Build}.{currentPackage.Id.Version.Revision}"; 

        }

        public static async Task<ApplicationInfo> GetAppInformation()
        {
            ApplicationInfo applicationInfo = new ApplicationInfo();

            var currentPackage = Windows.ApplicationModel.Package.Current;

            var appInstallerInfo = currentPackage.GetAppInstallerInfo();

            if (appInstallerInfo != null)
            {
                applicationInfo = await CollectApplicationInfo(appInstallerInfo.Uri.ToString());

                applicationInfo.updatesAvailable = UpdatesAvailable(applicationInfo);
            }
            else
            {
                applicationInfo.appCurrentVersion = GetCurrentAppVersion();

                applicationInfo.updatesAvailable = false;
            }

            return applicationInfo;

        }

    }
}
