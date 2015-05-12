using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ULocalizer.Classes
{
    /// <summary>
    /// Represents the translation node instances such as Variables,K2Node,SpriteCategory
    /// </summary>
    public class CTranslationNode : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private bool _isTopLevel = false;
        public bool isTopLevel
        {
            get { return _isTopLevel; }
            set { _isTopLevel = value; }
        }

        private string _IconPath = "/Images/variable.png";
        public string IconPath
        {
            get { return _IconPath; }
            set { _IconPath = value; NotifyPropertyChanged(); }
        }
        private string _Title = string.Empty;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// List of items to translate
        /// </summary>
        private CObservableList<CTranslationNodeItem> _Items = new CObservableList<CTranslationNodeItem>();
        public CObservableList<CTranslationNodeItem> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(); }
        }
        public override string ToString()
        {
            return this.Title;
        }
    }
}
