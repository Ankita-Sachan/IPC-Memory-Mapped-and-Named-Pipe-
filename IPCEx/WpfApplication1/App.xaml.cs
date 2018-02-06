using System.Windows;
using IOCContainer;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            log4net.Config.XmlConfigurator.Configure();
            Bootstrap.Start();
            MainWindow mainWindow = Bootstrap.GetInstance<MainWindow>();
            mainWindow.Show();
        }
    }
}
