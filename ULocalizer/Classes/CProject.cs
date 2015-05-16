using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ULocalizer.Binding;

namespace ULocalizer.Classes
{
    /// <summary>
    ///     Represents the localization project
    /// </summary>
    [Serializable]
    public class CProject : INotifyPropertyChanged
    {
        private string _destinationPath = "./Content/Localization/Game";
        private EncodingInfo _encoding = Common.Encodings.Find(e => e.Name == "utf-8");
        private bool _isChanged;
        private bool _isTranslationsChanged;
        private CObservableList<CCulture> _languages = new CObservableList<CCulture>();
        private string _name = string.Empty;
        private string _pathToEditor = string.Empty;
        private string _pathToProjectFile = string.Empty;
        private string _sourcePath = "./Content/Localization/Game";
        private CObservableList<CTranslation> _translations = new CObservableList<CTranslation>();
        private CCulture _sourceCulture = Common.Cultures.FirstOrDefault(culture => culture.ISO == "en");

        public string PathToEditor
        {
            get { return _pathToEditor; }
            set
            {
                _pathToEditor = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        public string PathToProjectFile
        {
            get { return _pathToProjectFile; }
            set
            {
                _pathToProjectFile = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        public EncodingInfo Encoding
        {
            get { return _encoding; }
            set
            {
                _encoding = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        public CCulture SourceCulture
        {
            get { return _sourceCulture; }
            set { _sourceCulture = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        public CObservableList<CCulture> Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {
                _destinationPath = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        [JsonIgnore]
        public CObservableList<CTranslation> Translations
        {
            get { return _translations; }
            set
            {
                _translations = value;
                NotifyPropertyChanged();
                IsTranslationsChanged = true;
            }
        }

        [JsonIgnore]
        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                _isChanged = value;
                NotifyPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool IsTranslationsChanged
        {
            get { return _isTranslationsChanged; }
            set
            {
                _isTranslationsChanged = value;
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

        public async Task Save()
        {
            var serializedProject = JsonConvert.SerializeObject(this);
            // ReSharper disable once AssignNullToNotNullAttribute
            await CUtils.SaveContentToFile(Path.Combine(Path.GetDirectoryName(PathToProjectFile), Name + ".ulp"), serializedProject, Encoding.GetEncoding());
        }

        public async Task SaveTranslations(bool clearTranslations, bool closeProgressAfterExecution = false)
        {
            await Task.Run(async () =>
            {
                await Common.ShowProgressMessage("Saving translations...", true);
                foreach (var translationInstance in Translations)
                {
                    if (File.Exists(translationInstance.Path))
                    {
                        try
                        {
                            var deserializedFile = JObject.Parse(File.ReadAllText(translationInstance.Path));
                            JToken tmpToken;
                            if (deserializedFile.TryGetValue("Namespace", out tmpToken))
                            {
                                deserializedFile["Namespace"] = string.Empty;
                            }
                            else
                            {
                                deserializedFile.Add("Namespace", string.Empty);
                            }
                            if (deserializedFile.TryGetValue("Children", out tmpToken))
                            {
                                deserializedFile["Children"] = new JArray();
                            }
                            else
                            {
                                deserializedFile.Add("Children", new JArray());
                            }
                            if (deserializedFile.TryGetValue("Subnamespaces", out tmpToken))
                            {
                                deserializedFile["Subnamespaces"] = new JArray();
                            }
                            else
                            {
                                deserializedFile.Add("Subnamespaces", new JArray());
                            }
                            foreach (var node in translationInstance.Nodes)
                            {
                                if (node.IsTopLevel)
                                {
                                    //deserializedFile.Value<JArray>("Children");
                                    var variablesNodeInst = JArray.FromObject(MakeChildList(node.Items));
                                    deserializedFile["Children"] = variablesNodeInst;
                                }
                                else
                                {
                                    var currentNodeJObject = new JObject {{"Namespace", node.Title}, {"Children", JArray.FromObject(MakeChildList(node.Items))}};
                                    deserializedFile.Value<JArray>("Subnamespaces").Add(currentNodeJObject);
                                }
                            }
                            await Task.Delay(100);
                            File.WriteAllText(translationInstance.Path, JsonConvert.SerializeObject(deserializedFile, Formatting.Indented));
                        }
                        catch (JsonException ex)
                        {
                            Common.WriteToConsole("Something wrong with " + translationInstance.Path + " or you don't have access to read this file." + ex.Message, MessageType.Error);
                        }
                    }
                    else
                    {
                        Common.WriteToConsole("Translation file " + translationInstance.Path + " doesn't exist.", MessageType.Error);
                    }
                }
                IsTranslationsChanged = false;
                if (clearTranslations)
                {
                    Translations = new CObservableList<CTranslation>();
                }
                if (closeProgressAfterExecution)
                {
                    await Common.ProgressController.CloseAsync();
                }
            });
        }

        private List<JObject> MakeChildList(CObservableList<CTranslationNodeItem> items)
        {
            var childItems = new List<JObject>();
            foreach (var item in items)
            {
                var newVarsJObject = new JObject();
                var sourceTextNode = new JObject();
                var translationTextNode = new JObject();
                sourceTextNode.Add("Text", item.Source);
                newVarsJObject.Add("Source", sourceTextNode);
                translationTextNode.Add("Text", item.Translation);
                newVarsJObject.Add("Translation", translationTextNode);
                childItems.Add(newVarsJObject);
            }
            return childItems;
        }

        public string GetProjectRoot()
        {
            return Path.GetDirectoryName(PathToProjectFile);
        }
    }
}