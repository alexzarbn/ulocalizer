using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ULocalizer.Binding;
namespace ULocalizer.Classes
{
    /// <summary>
    /// Represents the instance of translation item
    /// </summary>
    public class CTranslationNodeItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// Source text
        /// </summary>
        private string _Source = string.Empty;
        public string Source
        {
            get { return _Source; }
            set { _Source = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Translated text
        /// </summary>
        private string _Translation = string.Empty;
        public string Translation
        {
            get { return _Translation; }
            set { _Translation = value; NotifyPropertyChanged(); Projects.CurrentProject.isTranslationsChanged = true;  }
        }
    }
}
