using System;
using System.Linq;
using System.Windows;
using MahApps.Metro;
using ULocalizer.Binding;

namespace ULocalizer
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
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