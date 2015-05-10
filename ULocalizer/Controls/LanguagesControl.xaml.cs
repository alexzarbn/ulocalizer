using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ExtensionMethods;
using ULocalizer.Binding;
using ULocalizer.Classes;

namespace ULocalizer.Controls
{
    /// <summary>
    /// Interaction logic for LanguagesControl.xaml
    /// </summary>
    public partial class LanguagesControl : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public CObservableList<CultureInfo> DestinationSource
        {
            get { return (CObservableList<CultureInfo>)this.GetValue(DestinationSourceProperty); }
            set { this.SetValue(DestinationSourceProperty, value); NotifyPropertyChanged(); }
        }
        public static readonly DependencyProperty DestinationSourceProperty = DependencyProperty.Register(
          "DestinationSource", typeof(CObservableList<CultureInfo>), typeof(LanguagesControl), new PropertyMetadata(new CObservableList<CultureInfo>()));



        public CProject WorkingProject
        {
            get { return (CProject)GetValue(WorkingProjectProperty); }
            set { SetValue(WorkingProjectProperty, value); NotifyPropertyChanged(); }
        }
        public static readonly DependencyProperty WorkingProjectProperty =
            DependencyProperty.Register("WorkingProject", typeof(CProject), typeof(LanguagesControl), new PropertyMetadata(Projects.NewProject));



        public LanguagesControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void AddLangBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((SourceLanguagePicker.SelectedLanguage!=null) && (!WorkingProject.Languages.Contains(SourceLanguagePicker.SelectedLanguage)))
            {
                WorkingProject.Languages.Add(SourceLanguagePicker.SelectedLanguage);
                WorkingProject.Languages = WorkingProject.Languages.OrderBy(culture => culture.Name).ToObservableList();
            }
        }

        private void DelLangBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DestinationLanguagePicker.SelectedLanguage != null && DestinationLanguagePicker.Items.Count>1 && DestinationLanguagePicker.SelectedLanguage.Name!="en")
            {
                WorkingProject.Languages.Remove(DestinationLanguagePicker.SelectedLanguage);
            }
        }

        private void AddAllLangBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (CultureInfo CI in SourceLanguagePicker.Items)
            {
                if (!WorkingProject.Languages.Contains(CI))
                {
                    WorkingProject.Languages.Add(CI);
                }
            }
            WorkingProject.Languages = WorkingProject.Languages.OrderBy(culture => culture.Name).ToObservableList();
        }

        private void RemoveAllLangBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (CultureInfo CI in DestinationLanguagePicker.Items.ToList())
            {
                if ((CI.Name != "en") && (WorkingProject.Languages.Contains(CI)))
                {
                    WorkingProject.Languages.Remove(CI);
                }
            }
        }
    }
}
