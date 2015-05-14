using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
        private static CObservableList<CultureInfo> _cultures = new CObservableList<CultureInfo>();

        /// <summary>
        ///     Available cultures for automatic translation
        /// </summary>
        private static CObservableList<CultureInfo> _translationCultures = new CObservableList<CultureInfo>();

        private static string _consoleData = string.Empty;
        private static CTranslationNode _selectedNode;

        private static CTranslation _selectedTranslation;
        private static CTranslationNodeItem _selectedTranslationNodeItem;

        public static List<EncodingInfo> Encodings
        {
            get { return _encodings; }
            private set { _encodings = value; }
        }

        public static CObservableList<CultureInfo> Cultures
        {
            get { return _cultures; }
            private set { _cultures = value; }
        }

        public static CObservableList<CultureInfo> TranslationCultures
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
                Projects.XmlLang = XmlLanguage.GetLanguage(SelectedTranslation.Language.Name);
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
            Cultures.Clear(); //for sure...well, something can happend...it's programming, right ? x)
            Cultures.Add(CultureInfo.GetCultureInfo("de"));
            Cultures.Add(CultureInfo.GetCultureInfo("en"));
            Cultures.Add(CultureInfo.GetCultureInfo("es"));
            Cultures.Add(CultureInfo.GetCultureInfo("fr"));
            Cultures.Add(CultureInfo.GetCultureInfo("hi"));
            Cultures.Add(CultureInfo.GetCultureInfo("it"));
            Cultures.Add(CultureInfo.GetCultureInfo("ja"));
            Cultures.Add(CultureInfo.GetCultureInfo("ko"));
            Cultures.Add(CultureInfo.GetCultureInfo("pl"));
            Cultures.Add(CultureInfo.GetCultureInfo("pt"));
            Cultures.Add(CultureInfo.GetCultureInfo("ru"));
            Cultures.Add(CultureInfo.GetCultureInfo("sv"));
            Cultures.Add(CultureInfo.GetCultureInfo("zh"));
            await CTranslator.GetAvailableLanguages();
            foreach (var ci in Cultures.ToList())
            {
                AddRegions(ci.Name);
            }
            Cultures = Cultures.OrderBy(culture => culture.Name).ToObservableList();
        }

        private static void AddRegions(string parentCulture)
        {
            var regions = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.Parent.Name == parentCulture);
            foreach (var regionInstance in regions)
            {
                Cultures.Add(regionInstance);
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