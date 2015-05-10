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
                            JToken Subnamespaces;
                            CTranslationNode VariablesNode = null;
                            CTranslationNode K2Node = null;
                            CTranslationNode SpriteCategoryNode = null;
                            VariablesNode = TranslationInstance.Nodes.First(e => e.Title == "Variables");
                            K2Node = TranslationInstance.Nodes.First(e => e.Title == "K2Node");
                            SpriteCategoryNode = TranslationInstance.Nodes.First(e => e.Title == "SpriteCategory");
                            if (VariablesNode != null)
                            {
                                JArray VariablesNodeInst = DeserializedFile.Value<JArray>("Children");
                                VariablesNodeInst = JArray.FromObject(MakeChildList(VariablesNode.Items));
                                DeserializedFile["Children"] = VariablesNodeInst;
                            }
                            if ((K2Node != null) || (SpriteCategoryNode != null))
                            {
                                if (DeserializedFile.TryGetValue("Subnamespaces", out Subnamespaces))
                                {
                                    if (K2Node != null)
                                    {
                                        JToken K2NodeInst = Subnamespaces.Children().First(e => e.Value<string>("Namespace") == "K2Node");
                                        if (K2NodeInst != null)
                                        {
                                            JArray K2NodeInstChild = K2NodeInst.Value<JArray>("Children");
                                            K2NodeInstChild = JArray.FromObject(MakeChildList(K2Node.Items));
                                            K2NodeInst["Children"] = K2NodeInstChild;
                                        }
                                        else
                                        {
                                            Common.WriteToConsole("K2Node is broken or not found in " + TranslationInstance.Path + " file.",MessageType.Error);
                                        }
                                    }
                                    if (SpriteCategoryNode != null)
                                    {
                                        JToken SpriteCategoryNodeInst = Subnamespaces.Children().First(e => e.Value<string>("Namespace") == "SpriteCategory");
                                        if (SpriteCategoryNodeInst != null)
                                        {
                                            JArray SpriteCategoryNodeInstChild = SpriteCategoryNodeInst.Value<JArray>("Children");
                                            SpriteCategoryNodeInstChild = JArray.FromObject(MakeChildList(SpriteCategoryNode.Items));
                                            SpriteCategoryNodeInst["Children"] = SpriteCategoryNodeInstChild;
                                        }
                                        else
                                        {
                                            Common.WriteToConsole("Broken SpriteCategory node is broken or not found in " + TranslationInstance.Path + " file.",MessageType.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    Common.WriteToConsole("Cannot write K2Node and SpriteCategory nodes to " + TranslationInstance.Path + " file due to broken Subnamespaces node.", MessageType.Error);
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
                    this.Translations.Clear();
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
