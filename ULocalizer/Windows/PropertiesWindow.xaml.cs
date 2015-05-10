using System;
using ULocalizer.Classes;

namespace ULocalizer.Windows
{
    /// <summary>
    /// Interaction logic for PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow
    {
        public PropertiesWindow()
        {
            InitializeComponent();
        }

        private async void ProjectPropertiesControl_Executed(object sender, EventArgs e)
        {
            this.Close();
            await CBuilder.Build(true,true);
        }
    }
}
