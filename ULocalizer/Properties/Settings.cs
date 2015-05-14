using System.ComponentModel;

namespace ULocalizer.Properties
{
    internal sealed partial class Settings
    {
        private Settings()
        {
            PropertyChanged += NotifyPropertyChanged;
        }

        private void NotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Save();
        }
    }
}