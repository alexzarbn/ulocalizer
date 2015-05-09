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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;

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
