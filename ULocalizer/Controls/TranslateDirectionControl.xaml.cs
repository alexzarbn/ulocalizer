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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace ULocalizer.Controls
{
    /// <summary>
    /// Interaction logic for TranslateDirectionControl.xaml
    /// </summary>
    public partial class TranslateDirectionControl : UserControl
    {
        public TranslateDirectionControl()
        {
            InitializeComponent();
        }

        private void SwapBtn_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo SelectedSourceLanguage = SourceLanguage.SelectedLanguage;
            SourceLanguage.SelectedLanguage = DestinationLanguage.SelectedLanguage;
            DestinationLanguage.SelectedLanguage = SelectedSourceLanguage;
        }
    }
}
