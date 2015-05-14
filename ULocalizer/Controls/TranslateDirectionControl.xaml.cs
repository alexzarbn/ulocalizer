using System.Windows;

namespace ULocalizer.Controls
{
    /// <summary>
    ///     Interaction logic for TranslateDirectionControl.xaml
    /// </summary>
    public partial class TranslateDirectionControl
    {
        public TranslateDirectionControl()
        {
            InitializeComponent();
        }

        private void SwapBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedSourceLanguage = SourceLanguage.SelectedLanguage;
            SourceLanguage.SelectedLanguage = DestinationLanguage.SelectedLanguage;
            DestinationLanguage.SelectedLanguage = selectedSourceLanguage;
        }
    }
}