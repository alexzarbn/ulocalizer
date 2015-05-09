using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ULocalizer.Binding;
using ULocalizer.Windows;

namespace ULocalizer.Classes
{
    public static class CTranslator
    {
        private static RestClient Client = new RestClient("https://translate.yandex.net/api/v1.5/tr.json/");

        public static async Task GetAvailableLanguages()
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.TranslateAPIKey))
            {
                Common.WriteToConsole("Translate API key is not presented.");
                return;
            }
            RestRequest Request = new RestRequest("getLangs", Method.GET);
            Request.AddParameter("key", Properties.Settings.Default.TranslateAPIKey);
            Request.AddParameter("ui", "en");
            IRestResponse Response = null;
            try
            {
                Response = await Client.ExecuteGetTaskAsync(Request);
            }
            catch (Exception ex)
            {
                Common.WriteToConsole(ex.Message);
            }
            if ((Response!=null)  && (Response.ResponseStatus == ResponseStatus.Completed))
            {
                JObject ParsedResponse = JObject.Parse(Response.Content);
                foreach (JProperty LangInst in ParsedResponse.Value<JToken>("langs").Children())
                {
                   CultureInfo CI = CultureInfo.GetCultureInfo(LangInst.Name);
                   if (Common.Cultures.Contains(CI))
                   {
                       Common.TranslationCultures.Add(CI);
                   }
                }
                Common.WriteToConsole("List of languages available for translation has been loaded.");
            }
        }

        public static async Task TranslateProject(CultureInfo From, CultureInfo To)
        {
            await Common.ShowProgressMessage("Translating the whole project...");
            await Task.Delay(1000);
            foreach (CTranslation Translation in Projects.CurrentProject.Translations)
            {
                Translation.Nodes = await TranslateLanguage(Translation.Language.DisplayName,Translation.Nodes, From, To);
            }
            await Common.ProgressController.CloseAsync();
        }

        public static async Task TranslateLanguage(CTranslation SourceTranslation, CultureInfo From, CultureInfo To,bool closeProgress = false)
        {
            await Common.ShowProgressMessage("Translating "+SourceTranslation.Language.DisplayName+" language...");
            await Task.Delay(1000);
            foreach (CTranslationNode Node in SourceTranslation.Nodes)
            {
                Node.Items = await TranslateNode(Node.Title,Node.Items, From, To);
            }
            if (closeProgress)
            {
                await Common.ProgressController.CloseAsync();
            }
        }

        public static async Task<CObservableList<CTranslationNode>> TranslateLanguage(string Language,CObservableList<CTranslationNode> SourceNodes, CultureInfo From, CultureInfo To)
        {
            await Common.ShowProgressMessage("Translating " + Language + " language...");
            foreach (CTranslationNode Node in SourceNodes)
            {
                Node.Items = await TranslateNode(Node.Title,Node.Items, From, To);
            }
            return SourceNodes;
        }

        public static async Task TranslateNode(CTranslationNode SourceNode, CultureInfo From, CultureInfo To,bool closeProgress = false)
        {
            await Common.ShowProgressMessage("Translating " + SourceNode.Title + " node...");
            await Task.Delay(1000);
            SourceNode.Items = await TranslateNodeItems(SourceNode.Items, From, To);
            if (closeProgress)
            {
                await Common.ProgressController.CloseAsync();
            }
        }

        public static async Task<CObservableList<CTranslationNodeItem>> TranslateNode(string Title,CObservableList<CTranslationNodeItem> SourceNodeItems, CultureInfo From, CultureInfo To)
        {
            await Common.ShowProgressMessage("Translating " + Title + " node...");
            return await TranslateNodeItems(SourceNodeItems, From, To);
        }

        public static async Task TranslateList(CultureInfo From,CultureInfo To)
        {
            await Common.ShowProgressMessage("Translating selected items...");
            await Task.Delay(1000);
            foreach (CTranslationNodeItem Item in (App.Current.MainWindow as MainWindow).NodeItemsDataGrid.SelectedItems)
            {
                Item.Translation = await CTranslator.TranslateText(Item.Translation, From, To);
            }
            await Common.ShowProgressMessage("Selected items have been translated");
            await Common.ProgressController.CloseAsync();
        }


        public static async Task<CObservableList<CTranslationNodeItem>> TranslateNodeItems(CObservableList<CTranslationNodeItem> SourceText, CultureInfo From, CultureInfo To)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.TranslateAPIKey))
            {
                Common.WriteToConsole("Translate API key is not presented.");
                return SourceText;
            }
            RestRequest Request = new RestRequest("translate", Method.GET);
            Request.AddParameter("key", Properties.Settings.Default.TranslateAPIKey);
            Request.AddParameter("lang", From.Name+"-"+To.Name);
            foreach (CTranslationNodeItem Item in SourceText)
            {
                Request.AddParameter("text", Item.Translation);
            }
            IRestResponse Response = null;
            try
            {
                Response = await Client.ExecuteGetTaskAsync(Request);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            if ((Response!=null) && (Response.ResponseStatus == ResponseStatus.Completed))
            {
                JObject ParsedResponse = JObject.Parse(Response.Content);
                int counter = 0;
                foreach (CTranslationNodeItem Item in SourceText)
                {
                    Item.Translation = ParsedResponse.Value<JArray>("text")[counter].ToString();
                    counter++;
                }
                return SourceText;
            }
            else
            {
                return SourceText;
            }
        }

        public static async Task<string> TranslateText(string SourceText, CultureInfo From, CultureInfo To)
        {
            await Common.ShowProgressMessage("Translating " + SourceText);
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.TranslateAPIKey))
            {
                Common.WriteToConsole("Translate API key is not presented.");
                return SourceText;
            }
            RestRequest Request = new RestRequest("translate", Method.GET);
            Request.AddParameter("key", Properties.Settings.Default.TranslateAPIKey);
            Request.AddParameter("lang", From.Name + "-" + To.Name);
            IRestResponse Response = null;
            Request.AddParameter("text", SourceText);
            try
            {
                Response = await Client.ExecuteGetTaskAsync(Request);
            }
            catch (Exception ex)
            {
                Common.WriteToConsole(ex.Message);
            }
            if ((Response!=null) && (Response.ResponseStatus == ResponseStatus.Completed))
            {
                JObject ParsedResponse = JObject.Parse(Response.Content);
                return ParsedResponse.Value<JArray>("text")[0].ToString();
            }
            else
            {
                return SourceText;
            }
        }
      
    }
}
