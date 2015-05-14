using System.ComponentModel;
using System.Runtime.CompilerServices;
using ULocalizer.Binding;

namespace ULocalizer.Classes
{
    /// <summary>
    ///     Represents the instance of translation item
    /// </summary>
    public class CTranslationNodeItem : INotifyPropertyChanged
    {
        /// <summary>
        ///     Source text
        /// </summary>
        private string _source = string.Empty;

        /// <summary>
        ///     Translated text
        /// </summary>
        private string _translation = string.Empty;

        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                NotifyPropertyChanged();
            }
        }

        public string Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                NotifyPropertyChanged();
                Projects.CurrentProject.IsTranslationsChanged = true;
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