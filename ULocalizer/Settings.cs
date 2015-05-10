
namespace ULocalizer.Properties
{
    internal sealed partial class Settings
    {
        public Settings()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(NotifyPropertyChanged);

        }

        private void NotifyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Save();
        }
    }
}


