using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ULocalizer.Binding;

namespace ULocalizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Common.SetEncodings();
            Common.SetCultures();
#if !DEBUG
            System.Threading.Thread.Sleep(2000);
#endif
        }
    }
}
