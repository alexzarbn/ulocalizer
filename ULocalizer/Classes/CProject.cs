using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    /// Represents the localization project
    /// </summary>
    [Serializable]
    public class CProject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private string _PathToEditor = string.Empty;
        public string PathToEditor
        {
            get { return _PathToEditor; }
            set { _PathToEditor = value; NotifyPropertyChanged(); isChanged = true; }
        }
        private string _PathToProjectFile = string.Empty;
        public string PathToProjectFile
        {
            get { return _PathToProjectFile; }
            set { _PathToProjectFile = value; NotifyPropertyChanged(); isChanged = true; }
        }
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); isChanged = true; }
        }
        private EncodingInfo _Encoding = Binding.Common.Encodings.Find(e=> e.Name=="utf-8");
        public EncodingInfo Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value; NotifyPropertyChanged(); isChanged = true; }
        }
        private CObservableList<CultureInfo> _Languages = new CObservableList<CultureInfo>();
        public CObservableList<CultureInfo> Languages
        {
            get { return _Languages; }
            set { _Languages = value; NotifyPropertyChanged(); isChanged = true; }
        }
        private string _SourcePath = "./Content/Localization/Game";
        public string SourcePath
        {
            get { return _SourcePath; }
            set { _SourcePath = value; NotifyPropertyChanged(); isChanged = true; }
        }
        private string _DestinationPath = "./Content/Localization/Game";
        public string DestinationPath
        {
            get { return _DestinationPath; }
            set { _DestinationPath = value; NotifyPropertyChanged(); isChanged = true; }
        }
        private CObservableList<CTranslation> _Translations = new CObservableList<CTranslation>();
        [JsonIgnore]
        public CObservableList<CTranslation> Translations
        {
            get { return _Translations; }
            set { _Translations = value; NotifyPropertyChanged(); isTranslationsChanged = true; }
        }
        private bool _isChanged = false;
        [JsonIgnore]
        public bool isChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; NotifyPropertyChanged(); }
        }
        private bool _isTranslationsChanged = false;
        [JsonIgnore]
        public bool isTranslationsChanged
        {
            get { return _isTranslationsChanged; }
            set { _isTranslationsChanged = value; NotifyPropertyChanged(); }
        }
        public async Task Save()
        {
            string serializedProject = JsonConvert.SerializeObject(this);
            await CUtils.SaveContentToFile(Path.Combine(Path.GetDirectoryName(this.PathToProjectFile),this.Name + ".ulp"), serializedProject, this.Encoding.GetEncoding());
        }
        public async Task SaveTranslations(bool clearTranslations,bool closeProgressAfterExecution = false)
        {
            await Task.Run(async() =>
            {
                await Common.ShowProgressMessage("Saving translations...",true);
                foreach (CTranslation TranslationInstance in this.Translations)
                {
                    if (File.Exists(TranslationInstance.Path))
                    {
                        try
                        {
                            JObject DeserializedFile = JObject.Parse(File.ReadAllText(TranslationInstance.Path));
                            JToken tmpToken = null;
                            if (DeserializedFile.TryGetValue("Namespace", out tmpToken))
                            {
                                DeserializedFile["Namespace"] = string.Empty;
                            }
                            else
                            {
                                DeserializedFile.Add("Namespace",string.Empty);
                            }
                            tmpToken = null;
                            if (DeserializedFile.TryGetValue("Children", out tmpToken)) 
                            {
                                DeserializedFile["Children"] = new JArray();
                            }
                            else
                            {
                                DeserializedFile.Add("Children", new JArray());
                            }
                            tmpToken = null;
                            if (DeserializedFile.TryGetValue("Subnamespaces", out tmpToken))
                            {
                                DeserializedFile["Subnamespaces"] = new JArray();
                            }else{
                                DeserializedFile.Add("Subnamespaces",new JArray());
                            }
                            foreach (CTranslationNode Node in TranslationInstance.Nodes)
                            {
                                if (Node.isTopLevel)
                                {
                                    JArray VariablesNodeInst = DeserializedFile.Value<JArray>("Children");
                                    VariablesNodeInst = JArray.FromObject(MakeChildList(Node.Items));
                                    DeserializedFile["Children"] = VariablesNodeInst;
                                }
                                else
                                {
                                    JObject CurrentNodeJObject = new JObject();
                                    CurrentNodeJObject.Add("Namespace", Node.Title);
                                    CurrentNodeJObject.Add("Children", JArray.FromObject(MakeChildList(Node.Items)));
                                    DeserializedFile.Value<JArray>("Subnamespaces").Add(CurrentNodeJObject);
                                }
                            }
                            await Task.Delay(100);
                            File.WriteAllText(TranslationInstance.Path, JsonConvert.SerializeObject(DeserializedFile,Formatting.Indented));
                            
                        }
                        catch (JsonException ex)
                        {
                            Common.WriteToConsole("Something wrong with " + TranslationInstance.Path + " or you don't have access to read this file." + ex.Message,MessageType.Error);
                        }
                    }
                    else
                    {
                        Common.WriteToConsole("Translation file " + TranslationInstance.Path + " doesn't exist.",MessageType.Error);
                    }
                }
                this.isTranslationsChanged = false;
                if (clearTranslations)
                {
                    this.Translations = new CObservableList<CTranslation>();
                }
                if (closeProgressAfterExecution)
                {
                    await Common.ProgressController.CloseAsync();
                }
            });
        }
        private List<JObject> MakeChildList(CObservableList<CTranslationNodeItem> Items)
        {
            List<JObject> ChildItems = new List<JObject>();
            foreach (CTranslationNodeItem Item in Items)
            {
                JObject newVarsJObject = new JObject();
                JObject SourceTextNode = new JObject();
                JObject TranslationTextNode = new JObject();
                SourceTextNode.Add("Text", Item.Source);
                newVarsJObject.Add("Source", SourceTextNode);
                TranslationTextNode.Add("Text",Item.Translation);
                newVarsJObject.Add("Translation", TranslationTextNode);
                ChildItems.Add(newVarsJObject);
            }
            return ChildItems;
        }

        

        public string GetProjectRoot()
        {
            return Path.GetDirectoryName(this.PathToProjectFile);
        }
        public CProject()
        {
        }
    }
}
