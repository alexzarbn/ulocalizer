using System.Windows;
using ULocalizer.Binding;
using ULocalizer.Classes;

namespace ULocalizer.Windows
{
    /// <summary>
    /// Interaction logic for TranslateOptionsWindow.xaml
    /// </summary>
    public partial class TranslateOptionsWindow
    {
        private TranslateMode _Mode = TranslateMode.Project;
        public TranslateMode Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        public TranslateOptionsWindow()
        {
            InitializeComponent();
        }

        private async void TranslateBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            switch (Mode)
            {
                case TranslateMode.Project:
                    //await CTranslator.TranslateProject(DirectionsControl.SourceLanguage.SelectedLanguage, DirectionsControl.DestinationLanguage.SelectedLanguage);
                    break;
                case TranslateMode.Language:
                    await CTranslator.TranslateLanguage(Common.SelectedTranslation, DirectionsControl.SourceLanguage.SelectedLanguage, DirectionsControl.DestinationLanguage.SelectedLanguage,true);
                    break;
                case TranslateMode.Node:
                    await CTranslator.TranslateNode(Common.SelectedNode, DirectionsControl.SourceLanguage.SelectedLanguage, DirectionsControl.DestinationLanguage.SelectedLanguage,true);
                    break;
                case TranslateMode.Words:
                    await CTranslator.TranslateList(DirectionsControl.SourceLanguage.SelectedLanguage, DirectionsControl.DestinationLanguage.SelectedLanguage);
                    break;
                default:
                    await CTranslator.TranslateList(DirectionsControl.SourceLanguage.SelectedLanguage, DirectionsControl.DestinationLanguage.SelectedLanguage);
                    break;
            }
        }
    }
}
