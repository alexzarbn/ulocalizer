using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;
using ULocalizer.Classes;
using ULocalizer.Binding;
using MahApps.Metro.Controls.Dialogs;

namespace ULocalizer.Windows
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow
    {
        public NewProjectWindow()
        {
            InitializeComponent();
            Projects.NewProject = new CProject();
            Projects.NewProject.Languages.Add(CultureInfo.GetCultureInfo("en"));
        }

        private async void ProjectPropertiesControl_Executed(object sender, EventArgs e)
        {

            this.Close();
            await CBuilder.Build(true);
        }

    }
}
