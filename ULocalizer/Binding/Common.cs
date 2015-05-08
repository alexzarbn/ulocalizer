using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ULocalizer.Classes;
using ExtensionMethods;

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

        public static async Task ShowProgress(string Message)
        {
            if (!Common.IsProgressShown())
            {
                await Common.ShowProgressDialog(Message);
            }
            else
            {
                Common.ProgressController.SetMessage(Message);
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

        private static bool _isAvailable = true;
        /// <summary>
        /// Gets of sets the value of app activity
        /// </summary>
        public static bool isAvailable
        {
            get { return _isAvailable; }
            set { _isAvailable = value; RaiseStaticPropertyChanged("isAvailable"); }
        }

        private static CTranslationNode _SelectedNode = null;
        public static CTranslationNode SelectedNode
        {
            get { return _SelectedNode; }
            set { _SelectedNode = value; RaiseStaticPropertyChanged("SelectedNode"); }
        }

        private static CTranslation _SelectedLang = null;
        public static CTranslation SelectedLang
        {
            get { return _SelectedLang; }
            set { _SelectedLang = value; RaiseStaticPropertyChanged("SelectedLang"); Projects.XmlLang = XmlLanguage.GetLanguage(SelectedLang.Language.Name); }
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
        public static void SetCultures()
        {
            Cultures.Clear(); //for sure...well, something can happend...it's programming, right ?
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
        public static void WriteToConsole(string Text)
        {
            ConsoleData += Text + Environment.NewLine;
        }
    }
}
