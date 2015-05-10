using System.Globalization;
using System.Windows;
using System.Windows.Controls;

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
