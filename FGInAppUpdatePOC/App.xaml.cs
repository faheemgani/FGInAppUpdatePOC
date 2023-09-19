using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.UI.Xaml;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FGInAppUpdatePOC
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {

            AppCenter.Start("46a5e25a-473f-4a79-a43a-d3e4956a6254",
                  typeof(Analytics), typeof(Crashes));

            this.InitializeComponent();
            
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Current.UnhandledException += Current_UnhandledException;

            m_window = new MainWindow();
            m_window.Activate();

        }

        private Window m_window;

        private static void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Crashes.TrackError(e.Exception);
        }
    }
}
