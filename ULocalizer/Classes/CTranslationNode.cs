using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ULocalizer.Classes
{
    /// <summary>
    ///     Represents the translation node instances such as Variables,K2Node,SpriteCategory
    /// </summary>
    public class CTranslationNode : INotifyPropertyChanged
    {
        private string _iconPath = "/Images/variable.png";

        /// <summary>
        ///     List of items to translate
        /// </summary>
        private CObservableList<CTranslationNodeItem> _items = new CObservableList<CTranslationNodeItem>();

        private string _title = string.Empty;
        public bool IsTopLevel { get; set; }

        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                _iconPath = value;
                NotifyPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        public CObservableList<CTranslationNodeItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
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

        public override string ToString()
        {
            return Title;
        }
    }
}