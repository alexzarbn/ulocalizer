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
using System.Runtime.CompilerServices;
using System.Globalization;
using ULocalizer.Binding;
using ULocalizer.Classes;

namespace ULocalizer.Windows
{
    /// <summary>
    /// Interaction logic for LanguagesWindow.xaml
    /// </summary>
    public partial class LanguagesWindow : Window,INotifyPropertyChanged
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
        public LanguagesWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void AddLangBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Projects.CurrentProject.Languages.Contains(SelectedLanguage))
            {
                Projects.CurrentProject.Languages.Add(SelectedLanguage);
            }
        }

        private void DelLangBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLocalizedLanguage != null)
            {
                Projects.CurrentProject.Languages.Remove(SelectedLocalizedLanguage);
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            await Projects.CurrentProject.Save();
            Projects.isLanguagesListChanged = true;
            this.Close();
        }
    }
}
