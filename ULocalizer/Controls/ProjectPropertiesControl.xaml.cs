using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using ULocalizer.Binding;
using ULocalizer.Classes;
using ULocalizer.Properties;

namespace ULocalizer.Controls
{
    /// <summary>
    ///     Interaction logic for ProjectPropertiesControl.xaml
    /// </summary>
    public partial class ProjectPropertiesControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof (ProjectPropertiesMode), typeof (ProjectPropertiesControl), new PropertyMetadata(ProjectPropertiesMode.New));

        public ProjectPropertiesControl()
        {
            InitializeComponent();
        }

        public ProjectPropertiesMode Mode
        {
            private get { return (ProjectPropertiesMode) GetValue(ModeProperty); }
            set
            {
                SetValue(ModeProperty, value);
                NotifyPropertyChanged();
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

        public event EventHandler Executed;

        private void SetPathToEditorBtn_Click(object sender, RoutedEventArgs e)
        {
            ((CProject) DataContext).PathToEditor = CUtils.ShowFileDialog(FileTypesFilter.Executable);
        }

        private void SetPathToProjectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            ((CProject) DataContext).PathToProjectFile = CUtils.ShowFileDialog(FileTypesFilter.UProject);
            ((CProject) DataContext).Name = Path.GetFileNameWithoutExtension(((CProject) DataContext).PathToProjectFile);
        }

        private async void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((!string.IsNullOrWhiteSpace(((CProject) DataContext).DestinationPath)) && (!string.IsNullOrWhiteSpace(((CProject) DataContext).SourcePath)) && (!string.IsNullOrWhiteSpace(((CProject) DataContext).Name)) && (!string.IsNullOrWhiteSpace(((CProject) DataContext).PathToEditor)) && (!string.IsNullOrWhiteSpace(((CProject) DataContext).PathToProjectFile)) && (((CProject) DataContext).Languages.Count > 0))
            {
                if (SavePathField.IsChecked == true)
                {
                    Settings.Default.PathToEditor = ((CProject) DataContext).PathToEditor;
                }
                await ((CProject) DataContext).Save();
                if (Mode == ProjectPropertiesMode.New)
                {
                    Projects.CurrentProject = Projects.NewProject;
                }
            }
            else
            {
                MessageBox.Show("Some field(s) is empty", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (Executed != null)
            {
                Executed(this, EventArgs.Empty);
            }
        }

        private void UCProjectProperties_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.PathToEditor))
            {
                ((CProject) DataContext).PathToEditor = Settings.Default.PathToEditor;
            }
            if (Mode == ProjectPropertiesMode.Exist)
            {
                ActionBtn.Content = "Save";
            }
        }
    }
}