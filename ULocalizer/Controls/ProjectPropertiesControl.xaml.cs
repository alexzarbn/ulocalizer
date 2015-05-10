using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ULocalizer.Binding;
using ULocalizer.Classes;
namespace ULocalizer.Controls
{
    /// <summary>
    /// Interaction logic for ProjectPropertiesControl.xaml
    /// </summary>
    public partial class ProjectPropertiesControl : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public ProjectPropertiesMode Mode
        {
            get { return (ProjectPropertiesMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); NotifyPropertyChanged(); }
        }
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(ProjectPropertiesMode), typeof(ProjectPropertiesControl), new PropertyMetadata(ProjectPropertiesMode.New));
        public event EventHandler Executed;
        public ProjectPropertiesControl()
        {
            InitializeComponent();
        }
        private void SetPathToEditorBtn_Click(object sender, RoutedEventArgs e)
        {
            ((CProject)this.DataContext).PathToEditor = CUtils.ShowFileDialog(FileTypesFilter.Executable);
        }
        private void SetPathToProjectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            ((CProject)this.DataContext).PathToProjectFile = CUtils.ShowFileDialog(FileTypesFilter.UProject);
            ((CProject)this.DataContext).Name = System.IO.Path.GetFileNameWithoutExtension(((CProject)this.DataContext).PathToProjectFile);
        }
        private async void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (
                (!string.IsNullOrWhiteSpace(((CProject)this.DataContext).DestinationPath)) &&
                (!string.IsNullOrWhiteSpace(((CProject)this.DataContext).SourcePath)) &&
                (!string.IsNullOrWhiteSpace(((CProject)this.DataContext).Name)) &&
                (!string.IsNullOrWhiteSpace(((CProject)this.DataContext).PathToEditor)) &&
                (!string.IsNullOrWhiteSpace(((CProject)this.DataContext).PathToProjectFile)) &&
                (((CProject)this.DataContext).Languages.Count > 0)
            )
            {
                if (SavePathField.IsChecked == true)
                {
                    Properties.Settings.Default.PathToEditor = ((CProject)this.DataContext).PathToEditor;
                }
                await ((CProject)this.DataContext).Save();
                if (this.Mode == ProjectPropertiesMode.New)
                {
                    Projects.CurrentProject = Projects.NewProject;
                }
            }
            else
            {
                MessageBox.Show("Some field(s) is empty", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (this.Executed != null)
            {
                Executed(this, EventArgs.Empty);
            }
        }
        private void UCProjectProperties_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PathToEditor))
            {
                ((CProject)this.DataContext).PathToEditor = Properties.Settings.Default.PathToEditor;
            }
            if (this.Mode == ProjectPropertiesMode.Exist)
            {
                ActionBtn.Content = "Save";
            }
        }
        
    }
}
