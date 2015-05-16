namespace ULocalizer.Classes.Configuration
{
    public class CConfigSectionItem
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public CConfigSectionItem()
        {
            Key = string.Empty;
            Value = string.Empty;
        }
    }
}
