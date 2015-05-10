using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ULocalizer.Classes;
namespace ULocalizer.Controls
{
    /// <summary>
    /// Interaction logic for LanguagePickerControl.xaml
    /// </summary>
    public partial class LanguagePickerControl : UserControl, INotifyPropertyChanged
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
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); NotifyPropertyChanged(); }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
          "Title", typeof(string), typeof(LanguagePickerControl), new PropertyMetadata(string.Empty));
        public CObservableList<CultureInfo> Items
        {
            get { return (CObservableList<CultureInfo>)this.GetValue(ItemsProperty); }
            set { this.SetValue(ItemsProperty, value); NotifyPropertyChanged(); }
        }
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
          "Items", typeof(CObservableList<CultureInfo>), typeof(LanguagePickerControl), new PropertyMetadata(new CObservableList<CultureInfo>()));
        public LanguagePickerControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
