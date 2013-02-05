using System;
using System.Windows;
using System.Windows.Controls;

namespace City
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.SetData(
                "DataDirectory", 
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }
    }
}
