using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using ULocalizer.Classes;

namespace ULocalizer.Controls
{
    /// <summary>
    ///     Interaction logic for LanguagePickerControl.xaml
    /// </summary>
    public partial class LanguagePickerControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof (string), typeof (LanguagePickerControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof (CObservableList<CultureInfo>), typeof (LanguagePickerControl), new PropertyMetadata(new CObservableList<CultureInfo>()));

        private CultureInfo _selectedLanguage;

        public LanguagePickerControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public CultureInfo SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                NotifyPropertyChanged();
            }
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
                NotifyPropertyChanged();
            }
        }

        public CObservableList<CultureInfo> Items
        {
            get { return (CObservableList<CultureInfo>) GetValue(ItemsProperty); }
            set
            {
                SetValue(ItemsProperty, value);
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
    }
}