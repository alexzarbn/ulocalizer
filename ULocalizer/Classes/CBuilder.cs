using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;
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
        public static async Task Build()
        {
            await Task.Run(async() =>
            {
                Common.isAvailable = false;
                Common.ProcessText = "Building...";
                if (Common.WorkspaceVisibility == System.Windows.Visibility.Collapsed)
                {
                    Common.ToggleWorkspace();
                }
                Common.ToggleProcess();
                if (Common.OverlayVisibility == System.Windows.Visibility.Collapsed)
                {
                    Common.ToggleOverlay();
                }
                bool isSuccessfull = true;
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini")))
                {
                    try
                    {
                        File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini"), Path.Combine(Path.GetDirectoryName(Projects.CurrentProject.PathToProjectFile), @"Config\Localization.ini"), true);
                        List<string> CulturesToGenerate = new List<string>();
                        foreach (CultureInfo CI in Projects.CurrentProject.Languages)
                        {
                            CulturesToGenerate.Add("CulturesToGenerate="+CI.Name);
                        }
                        await CUtils.MakeConfig(Path.Combine(Path.GetDirectoryName(Projects.CurrentProject.PathToProjectFile), @"Config\Localization.ini"),CulturesToGenerate,Projects.CurrentProject.SourcePath,Projects.CurrentProject.DestinationPath);
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
                            Common.ConsoleData += BuilderProcess.StandardOutput.ReadLine() + Environment.NewLine;
                        }
                        while (!BuilderProcess.StandardError.EndOfStream)
                        {
                            isSuccessfull = false;
                            Common.ConsoleData += BuilderProcess.StandardError.ReadLine() + Environment.NewLine;
                        }
                        BuilderProcess.WaitForExit();
                        if (isSuccessfull)
                        {
                            await BuildTranslations();
                        }
                    }
                    catch (IOException ex)
                    {
                        isSuccessfull = false;
                        Common.WriteToConsole(ex.Message);
                    }
                }
                else
                {
                    Common.WriteToConsole("[ERROR] Default localization config doesn't exist.");
                    isSuccessfull = false;
                } 
                Common.isAvailable = true;
                Common.ToggleProcess();
                if (isSuccessfull)
                {
                    Common.SuccessText = "Build successful";
                    Common.ToggleSuccessText();
                }
                else
                {
                    Common.ErrorText = "Build error. See console for details.";
                    Common.ToggleErrorText();
                }
                await Task.Delay(2000);
                Common.ToggleOverlay();
                if (isSuccessfull)
                {
                    if (Common.WorkspaceVisibility == System.Windows.Visibility.Collapsed)
                    {
                        Common.ToggleWorkspace();
                    }
                }
            });

        }

        /// <summary>
        /// Builds the localization tree (on the left side of app)
        /// </summary>
        /// <returns></returns>
        public static async Task BuildTranslations()
        {
            await Task.Run(async() =>
            {
                Common.ProcessText = "Building translations...";
                Common.ToggleProcess();
                if (Common.OverlayVisibility == System.Windows.Visibility.Collapsed)
                {
                    Common.ToggleOverlay();
                }
                await Projects.CurrentProject.SaveTranslations(true);
                foreach (CultureInfo Lang in Projects.CurrentProject.Languages)
                {
                    if (Directory.Exists(Path.Combine(Projects.CurrentProject.GetProjectRoot(), Projects.CurrentProject.SourcePath)))
                    {
                        string[] Files = Directory.GetFiles(CUtils.FixPath(Path.Combine(Projects.CurrentProject.GetProjectRoot(), Projects.CurrentProject.SourcePath,Lang.Name)), "*.archive");
                        if (Files.Count() > 0)
                        {
                            try
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
                                                Item.Source = Val.Value<JToken>("Source").Value<string>("Text") ;
                                                Item.Translation = Val.Value<JToken>("Translation").Value<string>("Text");

                                                SubnamespacesNode.Items.Add(Item);
                                            }
                                            TranslationInstance.Nodes.Add(SubnamespacesNode);
                                            
                                        }
                                    }
                                    TranslationInstance.Nodes.Reverse();
                                    Projects.CurrentProject.Translations.Add(TranslationInstance);
                                }
                                else
                                {
                                    Common.WriteToConsole("Something wrong with translation file. Please, run repair using Tools->Repair.");
                                }
                            }
                            catch (JsonException ex)
                            {
                                Common.WriteToConsole(ex.Message);
                            }
                        }
                        else
                        {
                            Common.WriteToConsole("[ERROR] Translation file for " + Lang.DisplayName + " language doesn't exist.");
                        }
                    }
                    else
                    {
                        Common.WriteToConsole("[ERROR] Folder for " + Lang.DisplayName + " language doesn't exist.");
                    }
                }
                Projects.CurrentProject.isTranslationsChanged = false;
                Common.ToggleProcess();
            });
        }
    }
}
