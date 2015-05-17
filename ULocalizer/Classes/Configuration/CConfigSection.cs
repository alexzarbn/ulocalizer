using System.Linq;

namespace ULocalizer.Classes.Configuration
{
    public class CConfigSection
    {
        public CObservableList<CConfigSectionItem> Items { get; private set; } 

        public CConfigSection()
        {
            Items = new CObservableList<CConfigSectionItem>();
        }

        public void UpdateItem(string key,string value)
        {
            if (Items.FirstOrDefault(item => item.Key == key) != null)
            {
                Items.First(item => item.Key == key).Value = value;
            }
            else
            {
                Items.Add(new CConfigSectionItem {Key = key,Value = value});
            }
        }

        public void AddItem(string key, string value)
        {
            Items.Add(new CConfigSectionItem{Key = key,Value = value});
        }

    }
}
