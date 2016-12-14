using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ULocalizer.Binding;
using ULocalizer.Classes;

namespace ULocalizer.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly string _title = "ULocalizer";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string WTitle
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Projects.CurrentProject.Name))
                {
                    return _title;
                }
                return _title + " - " + Projects.CurrentProject.Name + ".ulp";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NewProjectWnd_Closed(object sender, EventArgs e)
        {
            var bindingExpression = GetBindingExpression(TitleProperty);
            if (bindingExpression != null) bindingExpression.UpdateTarget();
        }

        private async void BuildBtn_Click(object sender, RoutedEventArgs e)
        {
            await CBuilder.Build(true);
        }

        private void NewProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            var newProjectWnd = new NewProjectWindow {Owner = this};
            newProjectWnd.Closed += NewProjectWnd_Closed;
            newProjectWnd.ShowDialog();
        }

        private async void OpenProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            await Projects.Open();
            var bindingExpression = GetBindingExpression(TitleProperty);
            if (bindingExpression != null) bindingExpression.UpdateTarget();
        }

        private void LanguagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems.Count > 0) && (e.AddedItems[0].GetType().ToString() == "ULocalizer.Classes.CTranslation"))
            {
                Common.SelectedTranslation = (CTranslation) e.AddedItems[0];
                if (Common.SelectedTranslation.Nodes.Count > 0)
                {
                    NodeSelectionBtn.SelectedIndex = 0;
                }
            }
        }

        private async void SaveAllBtn_Click(object sender, RoutedEventArgs e)
        {
            await Projects.CurrentProject.SaveTranslations(false, true);
        }

        private void GitHubBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/aoki-sora/ulocalizer");
        }

        private void PropertiesBtn_Click(object sender, RoutedEventArgs e)
        {
            var propertiesWnd = new PropertiesWindow {Owner = this};
            propertiesWnd.ShowDialog();
        }

        private void ConsoleBtn_Click(object sender, RoutedEventArgs e)
        {
            var consoleWnd = new ConsoleWindow {Owner = this};
            consoleWnd.Show();
        }

        private async void ShowTranslateOptionsWindow(TranslateMode mode, bool hideDirectionControl = false)
        {
            if (Common.TranslationCultures.Any())
            {
                var translateOptionsWnd = new TranslateOptionsWindow {Owner = this, Mode = mode};
                if (hideDirectionControl)
                {
                    translateOptionsWnd.DirectionsControl.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Common.SelectedTranslation.Culture.ISO))
                {
                    translateOptionsWnd.DirectionsControl.SourceLanguage.SelectedLanguage = Common.TranslationCultures.First(translation => translation.ISO == (Projects.CurrentProject.SourceCulture.Parent != null ? Projects.CurrentProject.SourceCulture.Parent.ISO : Projects.CurrentProject.SourceCulture.ISO));
                    translateOptionsWnd.DirectionsControl.DestinationLanguage.SelectedLanguage = Common.TranslationCultures.First(translation => translation.ISO == (Common.SelectedTranslation.Culture.Parent != null ? Common.SelectedTranslation.Culture.Parent.ISO : Common.SelectedTranslation.Culture.ISO));
                }
                translateOptionsWnd.ShowDialog();
            }
            else
            {
                await Common.ShowError("List of languages available for translation is empty");
            }
        }

        private void TranslateSelectedWordsBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowTranslateOptionsWindow(TranslateMode.Words);
        }

        private void TranslateSelectedNodeBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowTranslateOptionsWindow(TranslateMode.Node);
        }

        private void TranslateSelectedLangBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowTranslateOptionsWindow(TranslateMode.Language);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            var settingsWnd = new SettingsWindow {Owner = this};
            settingsWnd.ShowDialog();
        }

        private void SetToDefValBtn_Click(object sender, RoutedEventArgs e)
        {
            CDefaults.SetItem(Common.SelectedNode,Common.SelectedTranslationNodeItem);
        }

        private void SetNodeToDefValBtn_Click(object sender, RoutedEventArgs e)
        {
            CDefaults.SetNode(Common.SelectedNode);
        }

        private void SetLangToDefValBtn_Click(object sender, RoutedEventArgs e)
        {
            CDefaults.SetTranslation(Common.SelectedTranslation);
        }

        private async void TranslateProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            await CTranslator.TranslateProject();
        }
    }
}