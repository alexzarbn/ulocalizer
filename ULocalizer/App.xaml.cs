using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ULocalizer.Binding;
using ULocalizer.Classes;
using MahApps.Metro;

namespace ULocalizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThemeManager.AddAccent("DefaultAccent", new Uri("/Themes/DefaultAccent.xaml", UriKind.Relative));
            ThemeManager.ChangeAppStyle(this, ThemeManager.Accents.First(x => x.Name == "DefaultAccent"), ThemeManager.AppThemes.First(x => x.Name == "BaseLight"));
            Common.SetEncodings();
            Common.SetCultures();
            
#if !DEBUG
            System.Threading.Thread.Sleep(2000);
#endif
        }
    }
}
