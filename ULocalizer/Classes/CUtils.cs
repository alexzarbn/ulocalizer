using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ULocalizer.Classes
{
    public enum FileTypesFilter : byte
    {
        [Description("Apps|*.exe")]
        Executable = 0,
        [Description("Unreal Engine Project|*.uproject")]
        UProject = 1,
        [Description("ULocalizer Project|*.ulp")]
        ULocProject = 2
    }

    public enum ProjectPropertiesMode : byte
    {
        New = 0,
        Exist = 1
    }

    public enum TranslateMode : byte
    {
        Project = 0,
        Language = 1,
        Node = 2,
        Words = 3
    }

    public enum MessageType : byte
    {
        Blank = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public static class CUtils
    {
        /// <summary>
        /// Shows up file dialog and returns the path of selected file
        /// </summary>
        /// <param name="pFilter">File type filter</param>
        /// <returns></returns>
        public static string ShowFileDialog(FileTypesFilter pFilter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = GetEnumDescription(pFilter);
            
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                return dlg.FileName;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets description attribute value
        /// </summary>
        /// <param name="EnumInstance">Enum to get description attribute from</param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum EnumInstance)
        {
            FieldInfo fi = EnumInstance.GetType().GetField(EnumInstance.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return EnumInstance.ToString();
            }
        }

        /// <summary>
        /// Saves string data to file with specified encoding
        /// </summary>
        /// <param name="Path">Location of the file to save to</param>
        /// <param name="Content">Content to save</param>
        /// <param name="ContentEncoding">Content encoding</param>
        /// <returns></returns>
        public static async Task SaveContentToFile(string Path,string Content,Encoding ContentEncoding)
        {
            await Task.Run(() =>
            {
                try
                {
                    File.WriteAllText(Path, Content, ContentEncoding);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Updates localization config
        /// </summary>
        /// <param name="Path">Path to config file</param>
        /// <param name="Cultures">Cultures to generate</param>
        /// <param name="SourcePath">SourcePath property for config</param>
        /// <param name="DestinationPath">DestinationPath property for config</param>
        /// <returns></returns>
        public static async Task MakeConfig(string Path, List<string> Cultures,string SourcePath,string DestinationPath)
        {
            await Task.Run(() =>
            {
                try 
                {
                    List<string> ConfigContent = File.ReadAllLines(Path).ToList();
                    ConfigContent.Insert(2, "SourcePath=" + SourcePath);
                    ConfigContent.Insert(3, "DestinationPath=" + DestinationPath);
                    ConfigContent.InsertRange(10, Cultures);
                    File.WriteAllLines(Path, ConfigContent);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Just replaces few symbols of the path to be sure that it's in correct form
        /// </summary>
        /// <param name="Path">Source path string</param>
        /// <returns></returns>
        public static string FixPath(string Path)
        {
            return Path.Replace(".","").Replace("/",@"\");
        }
    }
}
