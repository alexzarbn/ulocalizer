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
