using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ULocalizer.Controls
{
    /// <summary>
    /// Interaction logic for LanguagesComboBox.xaml
    /// </summary>
    public partial class LanguagesComboBox : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); NotifyPropertyChanged(); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LanguagesComboBox), new PropertyMetadata(string.Empty));


        public CultureInfo SelectedLanguage
        {
            get { return (CultureInfo)GetValue(SelectedLanguageProperty); }
            set { SetValue(SelectedLanguageProperty, value); NotifyPropertyChanged(); }
        }

        // Using a DependencyProperty as the backing store for SelectedLanguage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedLanguageProperty =
            DependencyProperty.Register("SelectedLanguage", typeof(CultureInfo), typeof(LanguagesComboBox), new PropertyMetadata(CultureInfo.GetCultureInfo("en")));



        

        public LanguagesComboBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
