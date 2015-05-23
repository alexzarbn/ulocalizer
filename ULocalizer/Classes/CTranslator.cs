using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using RestSharp;
using ULocalizer.Binding;
using ULocalizer.Properties;
using ULocalizer.Windows;

namespace ULocalizer.Classes
{
    public static class CTranslator
    {
        private static readonly RestClient Client = new RestClient("https://translate.yandex.net/api/v1.5/tr.json/");

        public static async Task GetAvailableLanguages()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.TranslateAPIKey))
            {
                Common.WriteToConsole("Translate API key is not presented.", MessageType.Warning);
                return;
            }
            var request = new RestRequest("getLangs", Method.GET);
            request.AddParameter("key", Settings.Default.TranslateAPIKey);
            request.AddParameter("ui", "en");
            IRestResponse response = null;
            try
            {
                response = await Client.ExecuteGetTaskAsync(request);
            }
            catch (Exception ex)
            {
                Common.WriteToConsole(ex.Message, MessageType.Error);
            }
            if ((response != null) && (response.ResponseStatus == ResponseStatus.Completed))
            {
                var parsedResponse = JObject.Parse(response.Content);
                foreach (var ci in parsedResponse.Value<JToken>("langs").Children<JProperty>().Where(ci => Common.Cultures.FirstOrDefault(culture => culture.ISO == ci.Name) != null))
                {
                    Common.TranslationCultures.Add(Common.Cultures.First(culture => culture.ISO == ci.Name));
                }
                Common.WriteToConsole("List of languages available for translation has been loaded.", MessageType.Info);
            }
        }

        public static async Task TranslateProject()
        {
            var from = Common.TranslationCultures.First(translation => translation.ISO == (Projects.CurrentProject.SourceCulture.Parent != null ? Projects.CurrentProject.SourceCulture.Parent.ISO : Projects.CurrentProject.SourceCulture.ISO));
            await Common.ShowProgressMessage("Translating the whole project...", true);
            foreach (var translation in
                Projects.CurrentProject.Translations.Where(translation => (translation.Culture.Parent != null && translation.Culture.Parent.ISO != @from.ISO) || (translation.Culture.Parent == null && translation.Culture.ISO != @from.ISO)))
            {
                translation.Nodes = await TranslateLanguage(translation.Culture.DisplayName, translation.Nodes, @from, translation.Culture.Parent ?? translation.Culture);
            }
            await Common.ProgressController.CloseAsync();
        }

        public static async Task TranslateLanguage(CTranslation sourceTranslation, CCulture @from, CCulture to, bool closeProgress = false)
        {
            await Common.ShowProgressMessage("Translating " + sourceTranslation.Culture.DisplayName + " language...", true);
            foreach (var node in sourceTranslation.Nodes)
            {
                node.Items = await TranslateNode(node.Title, node.Items, @from, to);
            }
            if (closeProgress)
            {
                await Common.ProgressController.CloseAsync();
            }
        }

        private static async Task<CObservableList<CTranslationNode>> TranslateLanguage(string language, CObservableList<CTranslationNode> sourceNodes, CCulture @from, CCulture to)
        {
            await Common.ShowProgressMessage("Translating " + language + " language...", false);
            foreach (var node in sourceNodes)
            {
                node.Items = await TranslateNode(node.Title, node.Items, @from, to);
            }
            return sourceNodes;
        }

        public static async Task TranslateNode(CTranslationNode sourceNode, CCulture @from, CCulture to, bool closeProgress = false)
        {
            await Common.ShowProgressMessage("Translating " + sourceNode.Title + " node...", true);
            sourceNode.Items = await TranslateNodeItems(sourceNode.Items, @from, to);
            if (closeProgress)
            {
                await Common.ProgressController.CloseAsync();
            }
        }

        private static async Task<CObservableList<CTranslationNodeItem>> TranslateNode(string title, CObservableList<CTranslationNodeItem> sourceNodeItems, CCulture @from, CCulture to)
        {
            await Common.ShowProgressMessage("Translating " + title + " node...", false);
            return await TranslateNodeItems(sourceNodeItems, @from, to);
        }

        public static async Task TranslateList(CCulture @from, CCulture to)
        {
            await Common.ShowProgressMessage("Translating selected items...", true);
            foreach (CTranslationNodeItem item in
                ((MainWindow) Application.Current.MainWindow).NodeItemsDataGrid.SelectedItems)
            {
                item.Translation = await TranslateText(item.Translation, @from, to);
            }
            await Common.ShowProgressMessage("Selected items have been translated", false);
            await Common.ProgressController.CloseAsync();
        }

        private static async Task<CObservableList<CTranslationNodeItem>> TranslateNodeItems(CObservableList<CTranslationNodeItem> sourceText, CCulture @from, CCulture to)
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.TranslateAPIKey))
            {
                Common.WriteToConsole("Translate API key is not presented.", MessageType.Error);
                return sourceText;
            }
            var request = new RestRequest("translate", Method.GET);
            request.AddParameter("key", Settings.Default.TranslateAPIKey);
            request.AddParameter("lang", @from.ISO + "-" + to.ISO);
            foreach (var item in sourceText)
            {
                request.AddParameter("text", item.Translation);
            }
            IRestResponse response = null;
            try
            {
                response = await Client.ExecuteGetTaskAsync(request);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            if ((response != null) && (response.ResponseStatus == ResponseStatus.Completed))
            {
                var parsedResponse = JObject.Parse(response.Content);
                var counter = 0;
                foreach (var item in sourceText)
                {
                    item.Translation = parsedResponse.Value<JArray>("text")[counter].ToString();
                    counter++;
                }
                return sourceText;
            }
            return sourceText;
        }

        private static async Task<string> TranslateText(string sourceText, CCulture @from, CCulture to)
        {
            await Common.ShowProgressMessage("Translating " + sourceText, false);
            if (string.IsNullOrWhiteSpace(Settings.Default.TranslateAPIKey))
            {
                Common.WriteToConsole("Translate API key is not presented.", MessageType.Error);
                return sourceText;
            }
            var request = new RestRequest("translate", Method.GET);
            request.AddParameter("key", Settings.Default.TranslateAPIKey);
            request.AddParameter("lang", @from.ISO + "-" + to.ISO);
            IRestResponse response = null;
            request.AddParameter("text", sourceText);
            try
            {
                response = await Client.ExecuteGetTaskAsync(request);
            }
            catch (Exception ex)
            {
                Common.WriteToConsole(ex.Message, MessageType.Error);
            }
            if ((response != null) && (response.ResponseStatus == ResponseStatus.Completed))
            {
                var parsedResponse = JObject.Parse(response.Content);
                return parsedResponse.Value<JArray>("text")[0].ToString();
            }
            return sourceText;
        }
    }
}