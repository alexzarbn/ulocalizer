using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Markup;
using Newtonsoft.Json;
using ULocalizer.Classes;

namespace ULocalizer.Binding
{
    public static class Projects
    {
        private static CProject _newProject = new CProject();
        private static CProject _currentProject = new CProject();
        private static XmlLanguage _xmlLang;
        private static bool _isProjectSetted;
        private static bool _isLanguagesListChanged;

        public static CProject NewProject
        {
            get { return _newProject; }
            set
            {
                _newProject = value;
                RaiseStaticPropertyChanged("NewProject");
            }
        }

        public static CProject CurrentProject
        {
            get { return _currentProject; }
            set
            {
                _currentProject = value;
                RaiseStaticPropertyChanged("CurrentProject");
                IsProjectSetted = true;
            }
        }

        public static XmlLanguage XmlLang
        {
            get { return _xmlLang; }
            set
            {
                _xmlLang = value;
                RaiseStaticPropertyChanged("XmlLang");
            }
        }

        public static bool IsProjectSetted
        {
            get { return _isProjectSetted; }
            private set
            {
                _isProjectSetted = value;
                RaiseStaticPropertyChanged("isProjectSetted");
            }
        }

        public static bool IsLanguagesListChanged
        {
            get { return _isLanguagesListChanged; }
            set
            {
                _isLanguagesListChanged = value;
                RaiseStaticPropertyChanged("isLanguagesListChanged");
            }
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void RaiseStaticPropertyChanged(string propName)
        {
            var handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }

        public static async Task Open()
        {
            await Task.Run(async () =>
            {
                var isSuccessful = true;
                var path = CUtils.ShowFileDialog(FileTypesFilter.ULocProject);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    try
                    {
                        var serializedProject = File.ReadAllText(path);
                        CurrentProject = JsonConvert.DeserializeObject<CProject>(serializedProject);
                        CurrentProject.IsChanged = false;
                        await CBuilder.LoadTranslations(true);
                        CurrentProject.IsTranslationsChanged = false;
                    }
                    catch (IOException ex)
                    {
                        isSuccessful = false;
                        Common.WriteToConsole(ex.Message, MessageType.Error);
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