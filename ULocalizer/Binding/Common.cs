using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using ExtensionMethods;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ULocalizer.Classes;

namespace ULocalizer.Binding
{
    /// <summary>
    /// Represents the common properties used in app
    /// </summary>
    public static class Common
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged(string propName)
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }

        private static List<EncodingInfo> _Encodings = new List<EncodingInfo>();
        public static List<EncodingInfo> Encodings
        {
            get { return _Encodings; }
            set { _Encodings = value; }
        }

        private static CObservableList<CultureInfo> _Cultures = new CObservableList<CultureInfo>();
        public static CObservableList<CultureInfo> Cultures
        {
            get { return _Cultures; }
            set { _Cultures = value; }
        }

        /// <summary>
        /// Available cultures for automatic translation
        /// </summary>
        private static CObservableList<CultureInfo> _TranslationCultures = new CObservableList<CultureInfo>();
        public static CObservableList<CultureInfo> TranslationCultures
        {
            get { return _TranslationCultures; }
            set { _TranslationCultures = value; }
        }


        private static ProgressDialogController _ProgressController = null;
        public static ProgressDialogController ProgressController
        {
            get { return _ProgressController; }
            set { _ProgressController = value; }
        }

        public static bool IsProgressShown()
        {
            if ((ProgressController!=null) && (ProgressController.IsOpen)) {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static async Task ShowProgressDialog(string Message)
        {
            await App.Current.Dispatcher.InvokeAsync(async () => ProgressController = await DialogManager.ShowProgressAsync(App.Current.MainWindow as MetroWindow, "Operation in progress", Message));
        }

        public static async Task ShowProgressMessage(string Message,bool useDelay)
        {
            if (!Common.IsProgressShown())
            {
                await Common.ShowProgressDialog(Message);
            }
            else
            {
                Common.ProgressController.SetMessage(Message);
            }
            if (useDelay)
            {
                await Task.Delay(1000);
            }
        }

        public static async Task ShowError(string Message)
        {
            await App.Current.Dispatcher.InvokeAsync(async () => await DialogManager.ShowMessageAsync(App.Current.MainWindow as MetroWindow, "Error",Message));
        }

        private static string _ConsoleData = string.Empty;
        public static string ConsoleData
        {
            get { return _ConsoleData; }
            set { _ConsoleData = value; RaiseStaticPropertyChanged("ConsoleData"); }
        }

        //private static bool _isAvailable = true;
        /// <summary>
        /// Gets of sets the value of app activity
        /// </summary>
        //public static bool isAvailable
        //{
        //    get { return _isAvailable; }
        //    set { _isAvailable = value; RaiseStaticPropertyChanged("isAvailable"); }
        //}

        private static CTranslationNode _SelectedNode = null;
        public static CTranslationNode SelectedNode
        {
            get { return _SelectedNode; }
            set { _SelectedNode = value; RaiseStaticPropertyChanged("SelectedNode"); }
        }

        private static CTranslation _SelectedTranslation = null;
        public static CTranslation SelectedTranslation
        {
            get { return _SelectedTranslation; }
            set { _SelectedTranslation = value; RaiseStaticPropertyChanged("SelectedTranslation"); Projects.XmlLang = XmlLanguage.GetLanguage(SelectedTranslation.Language.Name); }
        }

        private static CTranslationNodeItem _SelectedTranslationNodeItem = null;
        public static CTranslationNodeItem SelectedTranslationNodeItem
        {
            get { return _SelectedTranslationNodeItem; }
            set { _SelectedTranslationNodeItem = value; RaiseStaticPropertyChanged("SelectedTranslationNodeItem"); }
        }


        /// <summary>
        /// Sets the encodings list
        /// </summary>
        public static void SetEncodings()
        {
            EncodingInfo[] encodings = Encoding.GetEncodings();
            Encodings = encodings.ToList();
        }

        /// <summary>
        /// Sets the cultures list
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
            foreach (CultureInfo CI in Cultures.ToList())
            {
                AddRegions(CI.Name);
            }
            Cultures = Cultures.OrderBy(culture => culture.Name).ToObservableList();
        }

        private static void AddRegions(string ParentCulture)
        {
            var Regions = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.Parent.Name == ParentCulture);
            foreach (CultureInfo RegionInstance in Regions)
            {
                Cultures.Add(RegionInstance);
            }
        }

        /// <summary>
        /// Writes text to console
        /// </summary>
        /// <param name="Text">Text for writing</param>
        public static void WriteToConsole(string Text,MessageType messageType)
        {
            string Prefix = string.Empty;
            switch (messageType)
            {
                case MessageType.Blank:
                    break;
                case MessageType.Info:
                    Prefix = "[Info]";
                    break;
                case MessageType.Warning:
                    Prefix = "[Warning]";
                    break;
                case MessageType.Error:
                    Prefix = "[Error]";
                    break;
                default:
                    break;
            }
            if (messageType == MessageType.Info || messageType == MessageType.Warning || messageType == MessageType.Error)
            {
                Prefix += "[" + DateTime.Now.ToString() + "]: ";
            }
            ConsoleData += Prefix + Text + Environment.NewLine;
        }
    }
}
