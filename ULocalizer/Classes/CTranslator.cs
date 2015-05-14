using System;
using System.Globalization;
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
                foreach (var ci in from JProperty langInst in parsedResponse.Value<JToken>("langs").Children() select CultureInfo.GetCultureInfo(langInst.Name) into ci where Common.Cultures.Contains(ci) select ci)
                {
                    Common.TranslationCultures.Add(ci);
                }
                Common.WriteToConsole("List of languages available for translation has been loaded.", MessageType.Info);
            }
        }

        public static async Task TranslateProject(CultureInfo @from)
        {
            await Common.ShowProgressMessage("Translating the whole project...", true);
            foreach (var translation in
                Projects.CurrentProject.Translations.Where(translation => translation.Language.Name != @from.Name))
            {
                translation.Nodes = await TranslateLanguage(translation.Language.DisplayName, translation.Nodes, @from, string.IsNullOrWhiteSpace(translation.Language.Parent.Name) ? translation.Language : translation.Language.Parent);
            }
            await Common.ProgressController.CloseAsync();
        }

        public static async Task TranslateLanguage(CTranslation sourceTranslation, CultureInfo @from, CultureInfo to, bool closeProgress = false)
        {
            await Common.ShowProgressMessage("Translating " + sourceTranslation.Language.DisplayName + " language...", true);
            foreach (var node in sourceTranslation.Nodes)
            {
                node.Items = await TranslateNode(node.Title, node.Items, @from, to);
            }
            if (closeProgress)
            {
                await Common.ProgressController.CloseAsync();
            }
        }

        private static async Task<CObservableList<CTranslationNode>> TranslateLanguage(string language, CObservableList<CTranslationNode> sourceNodes, CultureInfo @from, CultureInfo to)
        {
            await Common.ShowProgressMessage("Translating " + language + " language...", false);
            foreach (var node in sourceNodes)
            {
                node.Items = await TranslateNode(node.Title, node.Items, @from, to);
            }
            return sourceNodes;
        }

        public static async Task TranslateNode(CTranslationNode sourceNode, CultureInfo @from, CultureInfo to, bool closeProgress = false)
        {
            await Common.ShowProgressMessage("Translating " + sourceNode.Title + " node...", true);
            sourceNode.Items = await TranslateNodeItems(sourceNode.Items, @from, to);
            if (closeProgress)
            {
                await Common.ProgressController.CloseAsync();
            }
        }

        private static async Task<CObservableList<CTranslationNodeItem>> TranslateNode(string title, CObservableList<CTranslationNodeItem> sourceNodeItems, CultureInfo @from, CultureInfo to)
        {
            await Common.ShowProgressMessage("Translating " + title + " node...", false);
            return await TranslateNodeItems(sourceNodeItems, @from, to);
        }

        public static async Task TranslateList(CultureInfo @from, CultureInfo to)
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

        private static async Task<CObservableList<CTranslationNodeItem>> TranslateNodeItems(CObservableList<CTranslationNodeItem> sourceText, CultureInfo @from, CultureInfo to)
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.TranslateAPIKey))
            {
                Common.WriteToConsole("Translate API key is not presented.", MessageType.Error);
                return sourceText;
            }
            var request = new RestRequest("translate", Method.GET);
            request.AddParameter("key", Settings.Default.TranslateAPIKey);
            request.AddParameter("lang", @from.Name + "-" + to.Name);
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

        private static async Task<string> TranslateText(string sourceText, CultureInfo @from, CultureInfo to)
        {
            await Common.ShowProgressMessage("Translating " + sourceText, false);
            if (string.IsNullOrWhiteSpace(Settings.Default.TranslateAPIKey))
            {
                Common.WriteToConsole("Translate API key is not presented.", MessageType.Error);
                return sourceText;
            }
            var request = new RestRequest("translate", Method.GET);
            request.AddParameter("key", Settings.Default.TranslateAPIKey);
            request.AddParameter("lang", @from.Name + "-" + to.Name);
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