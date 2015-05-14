using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace ULocalizer.Classes
{
    public enum FileTypesFilter : byte
    {
        [Description("Apps|*.exe")] Executable = 0,
        [Description("Unreal Engine Project|*.uproject")] UProject = 1,
        [Description("ULocalizer Project|*.ulp")] ULocProject = 2
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
        ///     Shows up file dialog and returns the path of selected file
        /// </summary>
        /// <param name="pFilter">File type filter</param>
        /// <returns></returns>
        public static string ShowFileDialog(FileTypesFilter pFilter)
        {
            var dlg = new OpenFileDialog {Filter = GetEnumDescription(pFilter)};

            var result = dlg.ShowDialog();

            if (result == true)
            {
                return dlg.FileName;
            }
            return null;
        }

        /// <summary>
        ///     Gets description attribute value
        /// </summary>
        /// <param name="enumInstance">Enum to get description attribute from</param>
        /// <returns></returns>
        private static string GetEnumDescription(Enum enumInstance)
        {
            var fi = enumInstance.GetType().GetField(enumInstance.ToString());
            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return enumInstance.ToString();
        }

        /// <summary>
        ///     Saves string data to file with specified encoding
        /// </summary>
        /// <param name="path">Location of the file to save to</param>
        /// <param name="content">Content to save</param>
        /// <param name="contentEncoding">Content encoding</param>
        /// <returns></returns>
        public static async Task SaveContentToFile(string path, string content, Encoding contentEncoding)
        {
            await Task.Run(() =>
            {
                try
                {
                    File.WriteAllText(path, content, contentEncoding);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        ///     Updates localization config
        /// </summary>
        /// <param name="path">Path to config file</param>
        /// <param name="cultures">Cultures to generate</param>
        /// <param name="sourcePath">SourcePath property for config</param>
        /// <param name="destinationPath">DestinationPath property for config</param>
        /// <returns></returns>
        public static async Task MakeConfig(string path, List<string> cultures, string sourcePath, string destinationPath)
        {
            await Task.Run(() =>
            {
                try
                {
                    var configContent = File.ReadAllLines(path).ToList();
                    configContent.Insert(2, "SourcePath=" + sourcePath);
                    configContent.Insert(3, "DestinationPath=" + destinationPath);
                    configContent.InsertRange(10, cultures);
                    File.WriteAllLines(path, configContent);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        ///     Just replaces few symbols of the path to be sure that it's in correct form
        /// </summary>
        /// <param name="path">Source path string</param>
        /// <returns></returns>
        public static string FixPath(string path)
        {
            return path.Replace(".", "").Replace("/", @"\");
        }
    }
}