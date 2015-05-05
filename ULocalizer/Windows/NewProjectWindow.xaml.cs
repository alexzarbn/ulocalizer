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
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using ULocalizer.Classes;
using ULocalizer.Binding;

namespace ULocalizer.Windows
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private CultureInfo _SelectedLanguage = null;
        public CultureInfo SelectedLanguage
        {
            get { return _SelectedLanguage; }
            set { _SelectedLanguage = value; NotifyPropertyChanged(); }
        }

        private CultureInfo _SelectedLocalizedLanguage = null;
        public CultureInfo SelectedLocalizedLanguage
        {
            get { return _SelectedLocalizedLanguage; }
            set { _SelectedLocalizedLanguage = value; NotifyPropertyChanged(); }
        }


        public NewProjectWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PathToEditor))
            {
                Projects.NewProject.PathToEditor = Properties.Settings.Default.PathToEditor;
            }
        }

        private void SetPathToEditorBtn_Click(object sender, RoutedEventArgs e)
        {
            Projects.NewProject.PathToEditor = CUtils.ShowFileDialog(FileTypesFilter.Executable);
        }

        private void SetPathToProjectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Projects.NewProject.PathToProjectFile = CUtils.ShowFileDialog(FileTypesFilter.UProject);
            Projects.NewProject.Name = System.IO.Path.GetFileNameWithoutExtension(Projects.NewProject.PathToProjectFile);
        }

        private void AddLangBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Projects.NewProject.Languages.Contains(SelectedLanguage))
            {
                Projects.NewProject.Languages.Add(SelectedLanguage);
            }
        }

        private void DelLangBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLocalizedLanguage != null)
            {
                Projects.NewProject.Languages.Remove(SelectedLocalizedLanguage);
            }
        }

        private async void CreateProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (
                (!string.IsNullOrWhiteSpace(Projects.NewProject.DestinationPath)) &&
                (!string.IsNullOrWhiteSpace(Projects.NewProject.SourcePath)) &&
                (!string.IsNullOrWhiteSpace(Projects.NewProject.Name)) &&
                (!string.IsNullOrWhiteSpace(Projects.NewProject.PathToEditor)) &&
                (!string.IsNullOrWhiteSpace(Projects.NewProject.PathToProjectFile)) &&
                (Projects.NewProject.Languages.Count > 0)
            )
            {
                if (SavePathField.IsChecked == true)
                {
                    Properties.Settings.Default.PathToEditor = Projects.NewProject.PathToEditor;
                }
                await Projects.NewProject.Save();
                Projects.CurrentProject = Projects.NewProject;
                Projects.isNewProjectCreated = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Some field(s) is empty", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
