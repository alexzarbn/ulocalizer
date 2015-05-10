using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Windows.Markup;
using Newtonsoft.Json;
using ULocalizer.Classes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ULocalizer.Binding
{
    public static class Projects
    {

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged(string propName)
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }

        private static CProject _NewProject = new CProject();
        public static CProject NewProject
        {
            get { return _NewProject; }
            set { _NewProject = value; RaiseStaticPropertyChanged("NewProject"); }
        }

        private static CProject _CurrentProject = new CProject();
        public static CProject CurrentProject
        {
            get { return _CurrentProject; }
            set { _CurrentProject = value; RaiseStaticPropertyChanged("CurrentProject"); isProjectSetted = true; }
        }

        private static XmlLanguage _XmlLang = null;
        public static XmlLanguage XmlLang
        {
            get { return _XmlLang; }
            set { _XmlLang = value; RaiseStaticPropertyChanged("XmlLang"); }
        }

        private static bool _isProjectSetted = false;
        public static bool isProjectSetted
        {
            get { return _isProjectSetted; }
            set { _isProjectSetted = value; RaiseStaticPropertyChanged("isProjectSetted"); }
        }

        private static bool _isLanguagesListChanged = false;
        public static bool isLanguagesListChanged
        {
            get { return _isLanguagesListChanged; }
            set { _isLanguagesListChanged = value; RaiseStaticPropertyChanged("isLanguagesListChanged"); }
        }


        public static async Task Open()
        {
            await Task.Run(async () =>
            {
                bool isSuccessful = true;
                string Path = CUtils.ShowFileDialog(FileTypesFilter.ULocProject);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    try
                    {
                        string serializedProject = File.ReadAllText(Path);
                        CurrentProject = JsonConvert.DeserializeObject<CProject>(serializedProject);
                        CurrentProject.isChanged = false;
                        await CBuilder.LoadTranslations(true);
                        CurrentProject.isTranslationsChanged = false;
                        //Common.isAvailable = true;
                    }
                    catch (IOException ex)
                    {
                        isSuccessful = false;
                        Common.WriteToConsole(ex.Message,MessageType.Error);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(CurrentProject.Name))
                    {
                        isSuccessful = false;
                    }
                }
                if (!isSuccessful)
                {
                    await Common.ShowError("Could not open the project. See console for details.");
                }
            });
        }

    }
}
