using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel;
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

        private static List<CultureInfo> _Cultures = new List<CultureInfo>();
        public static List<CultureInfo> Cultures
        {
            get { return _Cultures; }
            set { _Cultures = value; }
        }


        private static System.Windows.Visibility _OverlayVisibility = System.Windows.Visibility.Visible;
        public static System.Windows.Visibility OverlayVisibility
        {
            get { return _OverlayVisibility; }
            set { _OverlayVisibility = value; RaiseStaticPropertyChanged("OverlayVisibility"); }
        }

        private static System.Windows.Visibility _WorkspaceVisibility = System.Windows.Visibility.Collapsed;
        public static System.Windows.Visibility WorkspaceVisibility
        {
            get { return _WorkspaceVisibility; }
            set { _WorkspaceVisibility = value; RaiseStaticPropertyChanged("WorkspaceVisibility"); }
        }

        private static System.Windows.Visibility _WelcomeMessageVisibility = System.Windows.Visibility.Visible;
        public static System.Windows.Visibility WelcomeMessageVisibility
        {
            get { return _WelcomeMessageVisibility; }
            set { _WelcomeMessageVisibility = value; RaiseStaticPropertyChanged("WelcomeMessageVisibility"); }
        }

        private static System.Windows.Visibility _ProcessVisibility = System.Windows.Visibility.Collapsed;
        public static System.Windows.Visibility ProcessVisibility
        {
            get { return _ProcessVisibility; }
            set { _ProcessVisibility = value; RaiseStaticPropertyChanged("ProcessVisibility"); }
        }

        private static System.Windows.Visibility _SuccessTextVisibility = System.Windows.Visibility.Collapsed;
        public static System.Windows.Visibility SuccessTextVisibility
        {
            get { return _SuccessTextVisibility; }
            set { _SuccessTextVisibility = value; RaiseStaticPropertyChanged("SuccessTextVisibility"); }
        }

        private static System.Windows.Visibility _ErrorTextVisibility = System.Windows.Visibility.Collapsed;
        public static System.Windows.Visibility ErrorTextVisibility
        {
            get { return _ErrorTextVisibility; }
            set { _ErrorTextVisibility = value; RaiseStaticPropertyChanged("ErrorTextVisibility"); }
        }


        private static string _ProcessText = "Loading...";
        public static string ProcessText
        {
            get { return _ProcessText; }
            set { _ProcessText = value; RaiseStaticPropertyChanged("ProcessText"); }
        }

        private static string _SuccessText = "Completed";
        public static string SuccessText
        {
            get { return _SuccessText; }
            set { _SuccessText = value; RaiseStaticPropertyChanged("SuccessText"); }
        }

        private static string _ErrorText = "Unknown error";
        public static string ErrorText
        {
            get { return _ErrorText; }
            set { _ErrorText = value; RaiseStaticPropertyChanged("ErrorText"); }
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
            set { _SelectedLang = value; RaiseStaticPropertyChanged("SelectedLang"); }
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
            Cultures.Add(CultureInfo.GetCultureInfo("ja-jp"));
            Cultures.Add(CultureInfo.GetCultureInfo("ko-kr"));
            Cultures.Add(CultureInfo.GetCultureInfo("pl"));
            Cultures.Add(CultureInfo.GetCultureInfo("pt"));
            Cultures.Add(CultureInfo.GetCultureInfo("ru"));
            Cultures.Add(CultureInfo.GetCultureInfo("sv"));
            Cultures.Add(CultureInfo.GetCultureInfo("zh"));
            //CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            //Cultures = cultures.ToList();
            //Cultures.RemoveAt(0);
        }

        /// <summary>
        /// Writes text to console
        /// </summary>
        /// <param name="Text">Text for writing</param>
        public static void WriteToConsole(string Text)
        {
            ConsoleData += Text + Environment.NewLine;
        }

        public static void ToggleOverlay()
        {
            if (OverlayVisibility == System.Windows.Visibility.Visible)
            {
                OverlayVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                OverlayVisibility = System.Windows.Visibility.Visible;
            }
        }

        public static void ToggleWorkspace()
        {
            if (WorkspaceVisibility == System.Windows.Visibility.Collapsed)
            {
                WorkspaceVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                WorkspaceVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        public static void ToggleWelcomeMessage()
        {
            ProcessVisibility = System.Windows.Visibility.Collapsed;
            SuccessTextVisibility = System.Windows.Visibility.Collapsed;
            ErrorTextVisibility = System.Windows.Visibility.Collapsed;
            if (WelcomeMessageVisibility == System.Windows.Visibility.Collapsed)
            {
                WelcomeMessageVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                WelcomeMessageVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        public static void ToggleProcess()
        {
            WelcomeMessageVisibility = System.Windows.Visibility.Collapsed;
            SuccessTextVisibility = System.Windows.Visibility.Collapsed;
            ErrorTextVisibility = System.Windows.Visibility.Collapsed;
            if (ProcessVisibility == System.Windows.Visibility.Collapsed)
            {
                ProcessVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ProcessVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        public static void ToggleSuccessText()
        {
            WelcomeMessageVisibility = System.Windows.Visibility.Collapsed;
            ProcessVisibility = System.Windows.Visibility.Collapsed;
            ErrorTextVisibility = System.Windows.Visibility.Collapsed;
            if (SuccessTextVisibility == System.Windows.Visibility.Collapsed)
            {
                SuccessTextVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                SuccessTextVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        public static void ToggleErrorText()
        {
            WelcomeMessageVisibility = System.Windows.Visibility.Collapsed;
            ProcessVisibility = System.Windows.Visibility.Collapsed;
            SuccessTextVisibility = System.Windows.Visibility.Collapsed;
            if (ErrorTextVisibility == System.Windows.Visibility.Collapsed)
            {
                ErrorTextVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ErrorTextVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        public static void ToggleSaving()
        {
            ProcessText = "Saving...";
            ToggleProcess();
            ToggleOverlay();
        }
    }
}
