using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using ULocalizer.Binding;

namespace ULocalizer.Classes
{
    public class CTranslation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private CultureInfo _Language = null;
        public CultureInfo Language
        {
            get { return _Language; }
            set { _Language = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Path to translation file (*.archive)
        /// </summary>
        private string _Path =  string.Empty;
        public string Path
        {
            get { return _Path; }
            set { _Path = value; NotifyPropertyChanged(); }
        }

        public string IconPath
        {
            get { return "/Images/flags/" + this.Language.Name + ".png"; }
        }


        private CObservableList<CTranslationNode> _Nodes = new CObservableList<CTranslationNode>();
        public CObservableList<CTranslationNode> Nodes
        {
            get { return _Nodes; }
            set { _Nodes = value; NotifyPropertyChanged(); this.isChanged = true; }
        }

        private bool _isChanged = false;
        public bool isChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; NotifyPropertyChanged(); }
        }
        public override string ToString()
        {
            return this.Language.DisplayName;
        }
        
    }
}
