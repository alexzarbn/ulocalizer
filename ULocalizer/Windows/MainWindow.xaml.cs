using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ExtensionMethods;
using MahApps.Metro.Controls;
using ULocalizer.Binding;
using ULocalizer.Classes;

namespace ULocalizer.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _Title = "ULocalizer";
        public string WTitle
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(Projects.CurrentProject.Name))
                {
                    return _Title;
                }
                else
                {
                    return _Title + " - "+Projects.CurrentProject.Name + ".ulp";
                }
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;    
    
        }


        private void NewProjectWnd_Closed(object sender, EventArgs e)
        {
            this.GetBindingExpression(Window.TitleProperty).UpdateTarget();
        }
        
        private void LocDocs_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.unrealengine.com/latest/INT/Gameplay/Localization/index.html");
        }

        private async void BuildBtn_Click(object sender, RoutedEventArgs e)
        {
            await CBuilder.Build(true,false);
        }


        private void DefaultLocConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini")))
            {
                System.Diagnostics.Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini"));
            }
            else
            {
                Common.WriteToConsole("Cannot locate default localization config file.",MessageType.Error);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((e.NewValue != null) && (e.NewValue.GetType().ToString() == "ULocalizer.Classes.CTranslationNode"))
            {
                    Common.SelectedNode = (CTranslationNode)e.NewValue;
            }
        }

        public ItemsControl GetSelectedTreeViewItemParent(TreeView item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        private void LanguagesBtn_Click(object sender, RoutedEventArgs e)
        {
            PropertiesWindow PropertiesWnd = new PropertiesWindow();
            PropertiesWnd.Owner = this;
            PropertiesWnd.ShowDialog();
        }


        private void GettingStartedBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/aoki-sora/ulocalizer#getting-started");
        }

        private void NewProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            NewProjectWindow NewProjectWnd = new NewProjectWindow();
            NewProjectWnd.Owner = this;
            NewProjectWnd.Closed += NewProjectWnd_Closed;
            NewProjectWnd.ShowDialog();
        }

        private async void OpenProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            await Projects.Open();
            this.GetBindingExpression(Window.TitleProperty).UpdateTarget();
        }

        private void LanguagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems.Count>0) && (e.AddedItems[0].GetType().ToString() == "ULocalizer.Classes.CTranslation"))
            {
                Common.SelectedTranslation = (CTranslation)e.AddedItems[0];
                if (Common.SelectedTranslation.Nodes.Count > 0)
                {
                    NodeSelectionBtn.SelectedIndex = 0;
                }
            }
        }

        private async void SaveAllBtn_Click(object sender, RoutedEventArgs e)
        {
            await Projects.CurrentProject.SaveTranslations(true);
        }


        private void GitHubBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/aoki-sora/ulocalizer");
        }

        private void PropertiesBtn_Click(object sender, RoutedEventArgs e)
        {
            PropertiesWindow PropertiesWnd = new PropertiesWindow();
            PropertiesWnd.Owner = this;
            PropertiesWnd.ShowDialog();
        }

        private void ConsoleBtn_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWindow ConsoleWnd = new ConsoleWindow();
            ConsoleWnd.Owner = this;
            ConsoleWnd.Show();
        }

        private void SetToDefValBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Common.SelectedTranslationNodeItem != null)
            {
                try
                {
                    Common.SelectedTranslationNodeItem.Translation = Projects.CurrentProject.Translations.First(translation => translation.Language.Name == "en").Nodes.First(node => node.Title == Common.SelectedNode.Title).Items.First(item => item.Source == Common.SelectedTranslationNodeItem.Source).Translation;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }    
        }

        private async void ShowTranslateOptionsWindow(TranslateMode Mode,bool hideDirectionControl = false)
        {
            if (Common.TranslationCultures.Count > 1)
            {
                TranslateOptionsWindow TranslateOptionsWnd = new TranslateOptionsWindow();
                TranslateOptionsWnd.Owner = this;
                TranslateOptionsWnd.Mode = Mode;
                if (hideDirectionControl)
                {
                    TranslateOptionsWnd.DirectionsControl.Visibility = System.Windows.Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Common.SelectedTranslation.Language.Parent.Name))
                {
                    TranslateOptionsWnd.DirectionsControl.DestinationLanguage.SelectedLanguage = Common.TranslationCultures.First(translation => translation.Name == Common.SelectedTranslation.Language.Parent.Name);
                }
                TranslateOptionsWnd.ShowDialog();
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
            SettingsWindow SettingsWnd = new SettingsWindow();
            SettingsWnd.Owner = this;
            SettingsWnd.ShowDialog();
        }

        private void SetNodeToDefValBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Common.SelectedNode != null)
            {
                try
                {
                    Common.SelectedNode.Items = Projects.CurrentProject.Translations.First(translation => translation.Language.Name == "en").Nodes.First(node => node.Title == Common.SelectedNode.Title).Items.ToObservableList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }    
        }

        private void SetLangToDefValBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Common.SelectedTranslation != null)
            {
                try
                {
                    Common.SelectedTranslation.Nodes = Projects.CurrentProject.Translations.First(translation => translation.Language.Name == "en").Nodes.ToObservableList();
                    Common.SelectedNode = Common.SelectedTranslation.Nodes[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }    
        }

        private async void TranslateProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            await CTranslator.TranslateProject(Common.TranslationCultures.First(translation => translation.Name == "en"));
        }



    }
}
