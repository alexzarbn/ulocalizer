using System;
using System.Globalization;
using ULocalizer.Binding;
using ULocalizer.Classes;

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
            await CBuilder.Build(false,true);
        }
    }
}
