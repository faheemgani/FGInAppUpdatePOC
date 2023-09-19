using Microsoft.AppCenter.Crashes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;

namespace FGInAppUpdatePOC
{

    public sealed partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();

            Title = $"FGInAppUpdatePOC - {Utils.GetCurrentAppVersion()}";
        }

        private void ThrowErrorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.IO.File.ReadAllText("c:\\somenonexistentpathhere\\somefile.txt");
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            ApplicationInfo applicationInfo = await Utils.GetAppInformation();

            ContentDialog dialog = new ContentDialog
            {
                Title = "Checking for updates...",
                Content = $"Environment: {applicationInfo.appInstallerEnvironment} Current Version: {applicationInfo.appCurrentVersion} Latest Available Version: {applicationInfo.appInstallerMainPackageVersion} Updates Available: {applicationInfo.updatesAvailable}",

                CloseButtonText = "Cancel",
                PrimaryButtonText = "Update",

            };

            if (!applicationInfo.updatesAvailable)
                dialog.IsPrimaryButtonEnabled = false;

            dialog.PrimaryButtonClick += async (sender, args) =>
            {
                await Utils.TryToUpdateAsync(cancellationTokenSource.Token, applicationInfo);
            };

            dialog.XamlRoot = aboutBtn.XamlRoot;

            ContentDialogResult result = await dialog.ShowAsync();
        }
        
    }
}