using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using ULocalizer.Classes;

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

        private static bool _isNewProjectCreated = false;
        public static bool isNewProjectCreated
        {
            get { return _isNewProjectCreated; }
            set { _isNewProjectCreated = value; RaiseStaticPropertyChanged("isNewProjectCreated"); }
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
                Common.ProcessText = "Loading...";
                Common.ToggleProcess();
                if (Common.OverlayVisibility == System.Windows.Visibility.Collapsed)
                {
                    Common.ToggleOverlay();
                }
                string Path = CUtils.ShowFileDialog(FileTypesFilter.ULocProject);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    try
                    {
                        string serializedProject = File.ReadAllText(Path);
                        CurrentProject = JsonConvert.DeserializeObject<CProject>(serializedProject);
                        CurrentProject.isChanged = false;
                        Common.ToggleProcess();
                        await CBuilder.BuildTranslations();
                        CurrentProject.isTranslationsChanged = false;
                        Common.isAvailable = true;
                        Common.SuccessText = "The project has been loaded";
                        Common.ToggleSuccessText();
                        await Task.Delay(2000);
                        Common.ToggleSuccessText();
                    }
                    catch (IOException ex)
                    {
                        isSuccessful = false;
                        Common.WriteToConsole(ex.Message);
                        Common.ErrorText = "Cannot open the project.";
                        Common.ToggleErrorText();
                    }
                }
                else
                {
                    Common.ToggleProcess();
                    if (string.IsNullOrWhiteSpace(CurrentProject.Name))
                    {
                        isSuccessful = false;
                    }
                }
                if (isSuccessful)
                {
                    Common.ToggleOverlay();
                    if (Common.WorkspaceVisibility == System.Windows.Visibility.Collapsed)
                    {
                        Common.ToggleWorkspace();
                    }
                }
                else
                {
                    Common.ToggleErrorText();
                    Common.ToggleWelcomeMessage();
                }
            });
        }

    }
}
