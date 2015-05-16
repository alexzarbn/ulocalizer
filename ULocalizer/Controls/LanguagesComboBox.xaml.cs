using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ULocalizer.Binding;
using ULocalizer.Classes;

namespace ULocalizer.Controls
{
    /// <summary>
    ///     Interaction logic for LanguagesComboBox.xaml
    /// </summary>
    public partial class LanguagesComboBox : INotifyPropertyChanged
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof (string), typeof (LanguagesComboBox), new PropertyMetadata(string.Empty));
        // Using a DependencyProperty as the backing store for SelectedLanguage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedLanguageProperty = DependencyProperty.Register("SelectedLanguage", typeof (CCulture), typeof (LanguagesComboBox), new PropertyMetadata(Common.Cultures.FirstOrDefault(culture => culture.ISO == "en")));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof (CObservableList<CCulture>), typeof (LanguagesComboBox), new PropertyMetadata(default(CObservableList<CCulture>)));

        public CObservableList<CCulture> Items
        {
            get { return (CObservableList<CCulture>) GetValue(ItemsProperty); }
            set
            {
                SetValue(ItemsProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty UpdateLanguagesListProperty = DependencyProperty.Register("UpdateLanguagesList", typeof (bool), typeof (LanguagesComboBox), new PropertyMetadata(default(bool)));

        public bool UpdateLanguagesList
        {
            private get { return (bool) GetValue(UpdateLanguagesListProperty); }
            set
            {
                SetValue(UpdateLanguagesListProperty, value);
                NotifyPropertyChanged();
            }
        }

        public LanguagesComboBox()
        {
            InitializeComponent();
            DataContext = this;
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

        public CCulture SelectedLanguage
        {
            get { return (CCulture) GetValue(SelectedLanguageProperty); }
            set
            {
                SetValue(SelectedLanguageProperty, value);
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

        private void List_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UpdateLanguagesList)
            {
                if (Projects.IsProjectSetted)
                {
                    if (!Projects.CurrentProject.Languages.Contains(SelectedLanguage))
                    {
                        Projects.CurrentProject.Languages.Add(SelectedLanguage);
                        Projects.CurrentProject.SourceCulture = SelectedLanguage;
                    }
                }
                else
                {
                    if (!Projects.NewProject.Languages.Contains((SelectedLanguage)))
                    {
                        Projects.NewProject.Languages.Add(SelectedLanguage);
                        Projects.NewProject.SourceCulture = SelectedLanguage;
                    }
                }
            }
        }
    }
}