using Orimath.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Orimath
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ((RootViewModel)Resources["rootViewModel"]).Workspace.LoadSetting();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ((RootViewModel)Resources["rootViewModel"]).Workspace.SaveSetting();
        }
    }
}
