namespace ULocalizer.Classes.Configuration
{
    public class CConfigSection
    {
        public CObservableList<CConfigSectionItem> Items { get; set; } 

        public CConfigSection()
        {
            Items = new CObservableList<CConfigSectionItem>();
        }

    }
}
