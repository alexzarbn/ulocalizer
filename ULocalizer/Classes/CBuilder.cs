using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExtensionMethods;
using Newtonsoft.Json.Linq;
using ULocalizer.Binding;

namespace ULocalizer.Classes
{
    public static class CBuilder
    {
        /// <summary>
        /// Build the localization packages
        /// </summary>
        /// <returns></returns>
        public static async Task Build(bool saveTranslations,bool reloadTranslations)
        {
            await Task.Run(async () =>
            {
                if (saveTranslations)
                {
                    await Projects.CurrentProject.SaveTranslations(reloadTranslations);
                }
                await Common.ShowProgressMessage("Building...",false);
                bool isSuccessfull = true;
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini")))
                {
                    try
                    {
                        File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini"), Path.Combine(Path.GetDirectoryName(Projects.CurrentProject.PathToProjectFile), @"Config\Localization.ini"), true);
                        List<string> CulturesToGenerate = new List<string>();
                        foreach (CultureInfo CI in Projects.CurrentProject.Languages)
                        {
                            CulturesToGenerate.Add("CulturesToGenerate=" + CI.Name);
                        }
                        await CUtils.MakeConfig(Path.Combine(Path.GetDirectoryName(Projects.CurrentProject.PathToProjectFile), @"Config\Localization.ini"), CulturesToGenerate, Projects.CurrentProject.SourcePath, Projects.CurrentProject.DestinationPath);
                        var BuilderProcess = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = Projects.CurrentProject.PathToEditor,
                                Arguments = Projects.CurrentProject.PathToProjectFile + " -run=GatherText -config=" + Path.Combine(Path.GetDirectoryName(Projects.CurrentProject.PathToProjectFile), @"Config\Localization.ini"),
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                        };
                        BuilderProcess.Start();
                        while (!BuilderProcess.StandardOutput.EndOfStream)
                        {
                            Common.WriteToConsole(BuilderProcess.StandardOutput.ReadLine(), MessageType.Blank);
                        }
                        while (!BuilderProcess.StandardError.EndOfStream)
                        {
                            isSuccessfull = false;
                            Common.WriteToConsole(BuilderProcess.StandardError.ReadLine(), MessageType.Blank);
                        }
                        BuilderProcess.WaitForExit();
                        if (isSuccessfull && reloadTranslations)
                        {
                            await LoadTranslations(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        isSuccessfull = false;
                        Common.WriteToConsole(ex.Message,MessageType.Error);
                    }
                }
                else
                {
                    Common.WriteToConsole("Default localization config doesn't exist.",MessageType.Error);
                    isSuccessfull = false;
                }
                //Common.isAvailable = true;
                if (!isSuccessfull)
                {
                    await Common.ShowError("Build error. See console for details.");
                }
                await Common.ProgressController.CloseAsync();
            });
        }
        /// <summary>
        /// Builds the languages list (on the left side of app)
        /// </summary>
        /// <returns></returns>
        public static async Task LoadTranslations(bool closeProgressAfterExecution)
        {
            await Task.Run(async () =>
            {
                await Common.ShowProgressMessage("Loading translations...",true);
                Projects.CurrentProject.Translations.Clear();
                foreach (CultureInfo Lang in Projects.CurrentProject.Languages)
                {
                    if (Directory.Exists(Path.Combine(Projects.CurrentProject.GetProjectRoot(), Projects.CurrentProject.SourcePath)))
                    {
                        try
                        {
                            string[] Files = Directory.GetFiles(CUtils.FixPath(Path.Combine(Projects.CurrentProject.GetProjectRoot(), Projects.CurrentProject.SourcePath, Lang.Name)), "*.archive");
                            if (Files.Count() > 0)
                            {
                                JObject DeserializedTranslation = JObject.Parse(File.ReadAllText(Files[0]));
                                JToken Vars;
                                JToken Subnamespaces;
                                bool isVarsValid = DeserializedTranslation.TryGetValue("Children", out Vars);
                                bool isSubnamespacesValid = DeserializedTranslation.TryGetValue("Subnamespaces", out Subnamespaces);
                                if (isVarsValid || isSubnamespacesValid)
                                {
                                    CTranslation TranslationInstance = new CTranslation();
                                    TranslationInstance.Language = Lang;
                                    TranslationInstance.Path = Files[0];
                                    if (isVarsValid)
                                    {
                                        CTranslationNode VarsNode = new CTranslationNode();
                                        VarsNode.Title = "Variables";
                                        foreach (JToken Var in Vars.Children())
                                        {
                                            CTranslationNodeItem Item = new CTranslationNodeItem();
                                            Item.Source = Var.Value<JToken>("Source").Value<string>("Text");
                                            Item.Translation = Var.Value<JToken>("Translation").Value<string>("Text");
                                            VarsNode.Items.Add(Item);
                                        }
                                        TranslationInstance.Nodes.Add(VarsNode);
                                    }
                                    if (isSubnamespacesValid)
                                    {
                                        foreach (JToken Subnamespace in Subnamespaces)
                                        {
                                            CTranslationNode SubnamespacesNode = new CTranslationNode();
                                            SubnamespacesNode.Title = Subnamespace.Value<string>("Namespace");
                                            foreach (JToken Val in Subnamespace.Value<JToken>("Children").Children())
                                            {
                                                CTranslationNodeItem Item = new CTranslationNodeItem();
                                                Item.Source = Val.Value<JToken>("Source").Value<string>("Text");
                                                Item.Translation = Val.Value<JToken>("Translation").Value<string>("Text");
                                                SubnamespacesNode.Items.Add(Item);
                                            }
                                            TranslationInstance.Nodes.Add(SubnamespacesNode);
                                        }
                                    }
                                    Projects.CurrentProject.Translations.Add(TranslationInstance);
                                }
                                else
                                {
                                    Common.WriteToConsole("Something wrong with translation file. Please, run repair using Tools->Repair.",MessageType.Error);
                                }
                            }
                            else
                            {
                                Common.WriteToConsole("Translation file for " + Lang.DisplayName + " language doesn't exist.",MessageType.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.WriteToConsole(ex.Message,MessageType.Error);
                        }
                    }
                    else
                    {
                        Common.WriteToConsole("Folder for " + Lang.DisplayName + " language doesn't exist.",MessageType.Error);
                    }
                }
                await Common.ShowProgressMessage("Sorting...",true);
                Projects.CurrentProject.Translations = Projects.CurrentProject.Translations.OrderBy(translation => translation.Language.Name).ToObservableList();
                Projects.CurrentProject.isTranslationsChanged = false;
                if (closeProgressAfterExecution)
                {
                    await Common.ProgressController.CloseAsync();
                }
            });
        }
    }
}
