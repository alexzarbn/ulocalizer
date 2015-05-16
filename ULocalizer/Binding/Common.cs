using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ULocalizer.Classes;
using ULocalizer.ExtensionMethods;

namespace ULocalizer.Binding
{
    /// <summary>
    ///     Represents the common properties used in app
    /// </summary>
    public static class Common
    {
        private static List<EncodingInfo> _encodings = new List<EncodingInfo>();
        private static CObservableList<CCulture> _cultures = new CObservableList<CCulture>();

        /// <summary>
        ///     Available cultures for automatic translation
        /// </summary>
        private static CObservableList<CCulture> _translationCultures = new CObservableList<CCulture>();

        private static string _consoleData = string.Empty;
        private static CTranslationNode _selectedNode;
        private static CTranslation _selectedTranslation;
        private static CTranslationNodeItem _selectedTranslationNodeItem;

        public static List<EncodingInfo> Encodings
        {
            get { return _encodings; }
            private set { _encodings = value; }
        }

        public static CObservableList<CCulture> Cultures
        {
            get { return _cultures; }
            private set { _cultures = value; }
        }

        public static CObservableList<CCulture> TranslationCultures
        {
            get { return _translationCultures; }
            set { _translationCultures = value; }
        }

        public static ProgressDialogController ProgressController { get; private set; }

        public static string ConsoleData
        {
            get { return _consoleData; }
            private set
            {
                _consoleData = value;
                RaiseStaticPropertyChanged("ConsoleData");
            }
        }

        public static CTranslationNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                RaiseStaticPropertyChanged("SelectedNode");
            }
        }

        public static CTranslation SelectedTranslation
        {
            get { return _selectedTranslation; }
            set
            {
                _selectedTranslation = value;
                RaiseStaticPropertyChanged("SelectedTranslation");
                Projects.XmlLang = XmlLanguage.GetLanguage(SelectedTranslation.Culture.ISO);
            }
        }

        public static CTranslationNodeItem SelectedTranslationNodeItem
        {
            get { return _selectedTranslationNodeItem; }
            set
            {
                _selectedTranslationNodeItem = value;
                RaiseStaticPropertyChanged("SelectedTranslationNodeItem");
            }
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void RaiseStaticPropertyChanged(string propName)
        {
            var handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }

        private static bool IsProgressShown()
        {
            if ((ProgressController != null) && (ProgressController.IsOpen))
            {
                return true;
            }
            return false;
        }

        private static async Task ShowProgressDialog(string message)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () => ProgressController = await (Application.Current.MainWindow as MetroWindow).ShowProgressAsync("Operation in progress", message));
        }

        public static async Task ShowProgressMessage(string message, bool useDelay)
        {
            if (!IsProgressShown())
            {
                await ShowProgressDialog(message);
            }
            else
            {
                ProgressController.SetMessage(message);
            }
            if (useDelay)
            {
                await Task.Delay(1000);
            }
        }

        public static async Task ShowError(string message)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () => await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync("Error", message));
        }

        /// <summary>
        ///     Sets the encodings list
        /// </summary>
        public static void SetEncodings()
        {
            var encodings = Encoding.GetEncodings();
            Encodings = encodings.ToList();
        }

        /// <summary>
        ///     Sets the cultures list
        /// </summary>
        public static async void SetCultures()
        {
            Cultures.Clear();
            Cultures.Add(new CCulture {DisplayName = "German", ISO = "de"});
            Cultures.Add(new CCulture {DisplayName = "English", ISO = "en"});
            Cultures.Add(new CCulture {DisplayName = "Spanish", ISO = "es"});
            Cultures.Add(new CCulture {DisplayName = "Franch", ISO = "fr"});
            Cultures.Add(new CCulture {DisplayName = "Hindi", ISO = "hi"});
            Cultures.Add(new CCulture {DisplayName = "Italian", ISO = "it"});
            Cultures.Add(new CCulture {DisplayName = "Japanese", ISO = "ja"});
            Cultures.Add(new CCulture {DisplayName = "Korean", ISO = "ko"});
            Cultures.Add(new CCulture {DisplayName = "Polish", ISO = "pl"});
            Cultures.Add(new CCulture {DisplayName = "Portuguese", ISO = "pt"});
            Cultures.Add(new CCulture {DisplayName = "Russian", ISO = "ru"});
            Cultures.Add(new CCulture {DisplayName = "Swedish", ISO = "sv"});
            Cultures.Add(new CCulture {DisplayName = "Chinese", ISO = "zh"});
            Cultures.ToList().ForEach(culture => AddRegions(culture.ISO));
            await AddAdditionalCultures();
            await CTranslator.GetAvailableLanguages();
            Cultures = Cultures.OrderBy(culture => culture.ISO).ToObservableList();
        }

        private static void AddRegions(string parentCulture)
        {
            var regions = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.Parent.Name == parentCulture);
            foreach (var regionInstance in regions)
            {
                Cultures.Add(new CCulture {DisplayName = regionInstance.DisplayName, Parent = Cultures.First(culture => culture.ISO == regionInstance.Parent.Name), ISO = regionInstance.Name});
            }
        }

        private static async Task AddAdditionalCultures()
        {
            var isSuccessful = true;
            try
            {
                var additionalCulturesTextData = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\additionalCultures.txt"));
                if (additionalCulturesTextData.Any())
                {
                    foreach (var additionalCultureInfo in additionalCulturesTextData)
                    {
                        var additionalCultureSplitted = new[] {additionalCultureInfo.Substring(0, additionalCultureInfo.IndexOf(" ", StringComparison.Ordinal)), additionalCultureInfo.Substring(additionalCultureInfo.IndexOf(" ", StringComparison.Ordinal) + 1)};
                        var isoSplitted = additionalCultureSplitted[0].Split('-');
                        var parentCultureIso = isoSplitted[0];
                        var additionalCulture = new CCulture {ISO = additionalCultureSplitted[0]};
                        if (Cultures.FirstOrDefault(culture => culture.ISO == parentCultureIso) != null)
                        {
                            additionalCulture.Parent = Cultures.First(culture => culture.ISO == parentCultureIso);
                            additionalCulture.DisplayName = string.Join(" ", additionalCulture.Parent.DisplayName, additionalCultureSplitted[1]);
                        }
                        else
                        {
                            additionalCulture.DisplayName = additionalCultureSplitted[1];
                        }
                        Cultures.Add(additionalCulture);
                    }
                }
                else
                {
                    WriteToConsole("File with additional cultures is empty", MessageType.Error);
                    isSuccessful = false;
                }
            }
            catch (Exception ex)
            {
                WriteToConsole(ex.Message, MessageType.Error);
                isSuccessful = false;
            }
            if (!isSuccessful)
            {
                await ShowError("Additional cultures are not loaded. See console for details.");
            }
        }

        /// <summary>
        ///     Writes text to console
        /// </summary>
        /// <param name="text">Text for writing</param>
        /// <param name="messageType"></param>
        public static void WriteToConsole(string text, MessageType messageType)
        {
            var prefix = string.Empty;
            switch (messageType)
            {
                case MessageType.Blank:
                    break;
                case MessageType.Info:
                    prefix = "[Info]";
                    break;
                case MessageType.Warning:
                    prefix = "[Warning]";
                    break;
                case MessageType.Error:
                    prefix = "[Error]";
                    break;
            }
            if (messageType == MessageType.Info || messageType == MessageType.Warning || messageType == MessageType.Error)
            {
                prefix += "[" + DateTime.Now + "]: ";
            }
            ConsoleData += prefix + text + Environment.NewLine;
        }
    }
}