using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ULocalizer.Classes
{
    public class CTranslation : INotifyPropertyChanged
    {
        private bool _isChanged;
        private CultureInfo _language;
        private CObservableList<CTranslationNode> _nodes = new CObservableList<CTranslationNode>();

        /// <summary>
        ///     Path to translation file (*.archive)
        /// </summary>
        private string _path = string.Empty;

        public CultureInfo Language
        {
            get { return _language; }
            set
            {
                _language = value;
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
            get { return "/Images/flags/" + Language.Name + ".png"; }
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

        public bool IsChanged
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
            return Language.DisplayName;
        }
    }
}