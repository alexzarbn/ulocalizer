using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ULocalizer.Classes.Configuration
{
    public class CConfig
    {
        private string _path = string.Empty;

        private Dictionary<string, CConfigSection> Sections { get; set; }


        public CConfig()
        {
            Sections = new Dictionary<string, CConfigSection>();
        }


        public bool LoadFromFile(string path)
        {
            if (!File.Exists(path)) return false;
            _path = path;
            var fileContent = File.ReadAllLines(path);

            if (!fileContent.Any()) return false;

            var sections = Array.FindAll(fileContent, s => s.Trim().StartsWith("[") && s.EndsWith("]"));
            if (!sections.Any()) return false;

            for (var i = 0; i < sections.Length; i++)
            {
                var section = sections[i];
                var configSectionInstance = new CConfigSection();
                var index = Array.IndexOf(fileContent, section) + 1;
                var nextSessionIndex = 0;
                nextSessionIndex = i + 1 <= sections.Length - 1 ? Array.IndexOf(fileContent, sections[i + 1]) : fileContent.Length;
                while (index != nextSessionIndex)
                {
                    if ((string.IsNullOrWhiteSpace(fileContent[index])) || (!fileContent[index].Contains("=")))
                    {
                        index++;
                        continue;
                    }
                    var configSectionItemInstance = new CConfigSectionItem();
                    var itemSplitted = fileContent[index].Split('=');
                    configSectionItemInstance.Key = itemSplitted[0].Trim();
                    if (itemSplitted[1].Contains(';'))
                    {
                        itemSplitted[1] = itemSplitted[1].Substring(0, itemSplitted[1].IndexOf(';'));
                    }
                    configSectionItemInstance.Value = itemSplitted[1].Trim();
                    configSectionInstance.Items.Add(configSectionItemInstance);
                    index++;
                }
                Sections.Add(section.TrimStart('[').TrimEnd(']'), configSectionInstance);
            }
            return true;
        }
    }
}