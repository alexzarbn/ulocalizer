using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using ULocalizer.Binding;
using ULocalizer.ExtensionMethods;
using ULocalizer.Windows;

namespace ULocalizer.Classes
{
    public static class CBuilder
    {
        /// <summary>
        ///     Build the localization packages
        /// </summary>
        /// <returns></returns>
        public static async Task Build(bool saveTranslations)
        {
            await Task.Run(async () =>
            {
                if (saveTranslations)
                {
                    await Projects.CurrentProject.SaveTranslations(true);
                }
                await Common.ShowProgressMessage("Building...", false);
                var isSuccessfull = true;
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini")))
                {
                    try
                    {
                        if (Projects.CurrentProject.PathToProjectFile != null)
                        {
                            File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini"), Path.Combine(Path.GetDirectoryName(Projects.CurrentProject.PathToProjectFile), @"Config\Localization.ini"), true);
                            var culturesToGenerate = Projects.CurrentProject.Languages.Select(ci => "CulturesToGenerate=" + ci.Name).ToList();
                            if (Projects.CurrentProject.PathToProjectFile != null)
                            {
                                await CUtils.MakeConfig(Path.Combine(Path.GetDirectoryName(Projects.CurrentProject.PathToProjectFile), @"Config\Localization.ini"), culturesToGenerate, Projects.CurrentProject.SourcePath, Projects.CurrentProject.DestinationPath);
                                var builderProcess = new Process {StartInfo = new ProcessStartInfo {FileName = Projects.CurrentProject.PathToEditor, Arguments = Projects.CurrentProject.PathToProjectFile + " -run=GatherText -config=" + Path.Combine(Path.GetDirectoryName(Projects.CurrentProject.PathToProjectFile), @"Config\Localization.ini"), UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true}};
                                builderProcess.Start();
                                while (!builderProcess.StandardOutput.EndOfStream)
                                {
                                    Common.WriteToConsole(builderProcess.StandardOutput.ReadLine(), MessageType.Blank);
                                }
                                while (!builderProcess.StandardError.EndOfStream)
                                {
                                    isSuccessfull = false;
                                    Common.WriteToConsole(builderProcess.StandardError.ReadLine(), MessageType.Blank);
                                }
                                builderProcess.WaitForExit();
                            }
                        }
                        await Task.Delay(1000);
                        if (isSuccessfull)
                        {
                            await LoadTranslations(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        isSuccessfull = false;
                        Common.WriteToConsole(ex.Message, MessageType.Error);
                    }
                }
                else
                {
                    Common.WriteToConsole("Default localization config doesn't exist.", MessageType.Error);
                    isSuccessfull = false;
                }
                if (!isSuccessfull)
                {
                    await Common.ShowError("Build error. See console for details.");
                }
                await Common.ProgressController.CloseAsync();
            });
        }

        /// <summary>
        ///     Builds the languages list (on the left side of app)
        /// </summary>
        /// <returns></returns>
        public static async Task LoadTranslations(bool closeProgressAfterExecution)
        {
            await Task.Run(async () =>
            {
                await Common.ShowProgressMessage("Loading translations...", true);
                foreach (var lang in Projects.CurrentProject.Languages)
                {
                    if (Directory.Exists(Path.Combine(Projects.CurrentProject.GetProjectRoot(), Projects.CurrentProject.SourcePath)))
                    {
                        try
                        {
                            var files = Directory.GetFiles(CUtils.FixPath(Path.Combine(Projects.CurrentProject.GetProjectRoot(), Projects.CurrentProject.SourcePath, lang.Name)), "*.archive");
                            if (files.Any())
                            {
                                var deserializedTranslation = JObject.Parse(File.ReadAllText(files[0]));
                                JToken vars;
                                JToken subnamespaces;
                                var isVarsValid = deserializedTranslation.TryGetValue("Children", out vars);
                                var isSubnamespacesValid = deserializedTranslation.TryGetValue("Subnamespaces", out subnamespaces);
                                if (isVarsValid || isSubnamespacesValid)
                                {
                                    var translationInstance = new CTranslation {Language = lang, Path = files[0]};
                                    if (isVarsValid)
                                    {
                                        var varsNode = new CTranslationNode {IsTopLevel = true, Title = "Variables"};
                                        foreach (var var in vars.Children())
                                        {
                                            var item = new CTranslationNodeItem {Source = var.Value<JToken>("Source").Value<string>("Text"), Translation = var.Value<JToken>("Translation").Value<string>("Text")};
                                            varsNode.Items.Add(item);
                                        }
                                        translationInstance.Nodes.Add(varsNode);
                                    }
                                    if (isSubnamespacesValid)
                                    {
                                        foreach (var subnamespace in subnamespaces)
                                        {
                                            var subnamespacesNode = new CTranslationNode {Title = subnamespace.Value<string>("Namespace")};
                                            foreach (var val in subnamespace.Value<JToken>("Children").Children())
                                            {
                                                var item = new CTranslationNodeItem {Source = val.Value<JToken>("Source").Value<string>("Text"), Translation = val.Value<JToken>("Translation").Value<string>("Text")};
                                                subnamespacesNode.Items.Add(item);
                                            }
                                            translationInstance.Nodes.Add(subnamespacesNode);
                                        }
                                    }
                                    Projects.CurrentProject.Translations.Add(translationInstance);
                                }
                                else
                                {
                                    Common.WriteToConsole("Something wrong with translation file. Please, run repair using Tools->Repair.", MessageType.Error);
                                }
                            }
                            else
                            {
                                Common.WriteToConsole("Translation file for " + lang.DisplayName + " language doesn't exist.", MessageType.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.WriteToConsole(ex.Message, MessageType.Error);
                        }
                    }
                    else
                    {
                        Common.WriteToConsole("Folder for " + lang.DisplayName + " language doesn't exist.", MessageType.Error);
                    }
                }
                await Common.ShowProgressMessage("Sorting...", true);
                Projects.CurrentProject.Translations = Projects.CurrentProject.Translations.OrderBy(translation => translation.Language.Name).ToObservableList();
                Projects.CurrentProject.IsTranslationsChanged = false;
                await Application.Current.Dispatcher.InvokeAsync(() => ((MainWindow) Application.Current.MainWindow).LanguagesList.SelectedIndex = 0);
                if (closeProgressAfterExecution)
                {
                    await Common.ProgressController.CloseAsync();
                }
            });
        }
    }
}