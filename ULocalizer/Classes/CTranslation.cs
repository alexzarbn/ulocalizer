using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ULocalizer.Classes
{
    public class CTranslation : INotifyPropertyChanged
    {
        private CCulture _culture;
        private bool _isChanged;
        private CObservableList<CTranslationNode> _nodes = new CObservableList<CTranslationNode>();

        /// <summary>
        ///     Path to translation file (*.archive)
        /// </summary>
        private string _path = string.Empty;

        public CCulture Culture
        {
            get { return _culture; }
            set
            {
                _culture = value;
                NotifyPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                NotifyPropertyChanged();
            }
        }

        public string IconPath
        {
            get { return "/Images/flags/" + Culture.ISO + ".png"; }
        }

        public CObservableList<CTranslationNode> Nodes
        {
            get { return _nodes; }
            set
            {
                _nodes = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        private bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                _isChanged = value;
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
            return Culture.DisplayName;
        }
    }
}