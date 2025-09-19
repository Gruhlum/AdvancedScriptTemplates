using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    public static class CreateScriptMenu
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.delayCall += CreateMenus;
        }

        private static void CreateMenus()
        {
            if (TemplateSettings.instance.scriptTemplateDatas == null)
            {
                return;
            }
            foreach (ScriptTemplateData scriptTemplateData in TemplateSettings.instance.scriptTemplateDatas)
            {
                scriptTemplateData.VerifyMenu();
            }
        }


        [Shortcut("Script1", KeyCode.Alpha1, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        public static void GenerateScript1()
        {
            if (TemplateSettings.instance.scriptTemplateDatas.Count > 0)
            {
                CreateTemplate(TemplateSettings.instance.scriptTemplateDatas[0]);
            }
        }
        [Shortcut("Script2", KeyCode.Alpha2, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        public static void GenerateScript2()
        {
            if (TemplateSettings.instance.scriptTemplateDatas.Count > 1)
            {
                CreateTemplate(TemplateSettings.instance.scriptTemplateDatas[1]);
            }
        }
        [Shortcut("Script3", KeyCode.Alpha3, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        public static void GenerateScript3()
        {
            if (TemplateSettings.instance.scriptTemplateDatas.Count > 2)
            {
                CreateTemplate(TemplateSettings.instance.scriptTemplateDatas[2]);
            }
        }
        [Shortcut("Script4", KeyCode.Alpha4, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        public static void GenerateScript4()
        {
            if (TemplateSettings.instance.scriptTemplateDatas.Count > 3)
            {
                CreateTemplate(TemplateSettings.instance.scriptTemplateDatas[3]);
            }
        }
        [Shortcut("Script5", KeyCode.Alpha5, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        public static void GenerateScript5()
        {
            if (TemplateSettings.instance.scriptTemplateDatas.Count > 4)
            {
                CreateTemplate(TemplateSettings.instance.scriptTemplateDatas[4]);
            }
        }
        [Shortcut("Script6", KeyCode.Alpha6, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        public static void GenerateScript6()
        {
            if (TemplateSettings.instance.scriptTemplateDatas.Count > 5)
            {
                CreateTemplate(TemplateSettings.instance.scriptTemplateDatas[5]);
            }
        }

        [MenuItem("Assets/Regenerate Namespace", priority = 19, secondaryPriority = 1000)]
        public static void RegenerateNamespaces()
        {
            List<string> scriptFiles = FindAllScriptPaths();
            RegenerateNamespaces(scriptFiles);
        }

        private static List<string> FindAllScriptPaths()
        {
            Object[] selectedObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);

            List<string> scriptPaths = new List<string>();

            foreach (Object obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (path.EndsWith(".cs"))
                {
                    scriptPaths.Add(path.Replace('\\', '/'));
                }
                else
                {
                    List<string> results = GetScriptPathsFromFolder(path);
                    if (results != null)
                    {
                        scriptPaths.AddRange(results);
                    }
                }
            }

            return scriptPaths.Distinct().ToList();
        }

        private static List<string> GetScriptPathsFromFolder(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                return null;
            }
            List<string> scriptPaths = new List<string>();

            List<string> subDirectoryPaths = Directory.GetDirectories(folderPath).ToList();
            foreach (string subDirectoryPath in subDirectoryPaths)
            {
                List<string> results = GetScriptPathsFromFolder(subDirectoryPath);
                if (results != null)
                {
                    scriptPaths.AddRange(results);
                }
            }

            List<string> filePaths = Directory.GetFiles(folderPath).ToList();

            foreach (string result in filePaths)
            {
                if (result.EndsWith(".cs"))
                {
                    scriptPaths.Add(result.Replace('\\', '/'));
                }
            }

            return scriptPaths;
        }

        private static void RegenerateNamespaces(List<string> scriptPaths)
        {
            List<Object> scriptObjects = new List<Object>();
            foreach (string path in scriptPaths)
            {
                scriptObjects.Add(AssetDatabase.LoadAssetAtPath<Object>(path));
                //Undo.RecordObject(TemplateSettings.instance, "Namespace Fix");
            }
            Undo.RecordObjects(scriptObjects.ToArray(), "Namespace Fix");

            foreach (string path in scriptPaths)
            {
                //Debug.Log(path);
                RegenerateNamespace(path);
            }

            AssetDatabase.Refresh();
        }

        private static void RegenerateNamespace(string path)
        {
            string scriptText = File.ReadAllText(path);
            int nameSpaceStart = scriptText.IndexOf("namespace");
            int nameSpaceEnd = scriptText.Substring(nameSpaceStart).IndexOf(Environment.NewLine);
            string currentNameSpace = scriptText.Substring(nameSpaceStart, nameSpaceEnd);

            string fixedNameSpace = "namespace " + TemplateSettings.instance.GenerateNamespaceName(path);

            Debug.Log(currentNameSpace + " -> " + fixedNameSpace);

            scriptText = scriptText.Replace(currentNameSpace, fixedNameSpace);

            File.WriteAllText(path, scriptText);
        }

        public static void CreateTemplate(ScriptTemplateData templateData)
        {
            CreateScriptEndNameEditAction create = ScriptableObject.CreateInstance<CreateScriptEndNameEditAction>();

            create.templateData = templateData;
            string newPath = Path.Combine(GetFolderPath(), templateData.newFileName + ".cs");
            Texture2D icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, newPath, icon, null);
        }

        private static string GetFolderPath()
        {
            Object[] selectedObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);

            if ((selectedObjects?.Length ?? 0) > 0)
            {
                string folderPath = AssetDatabase.GetAssetPath(selectedObjects[0]);
                if (AssetDatabase.IsValidFolder(folderPath))
                {
                    return folderPath;
                }
                else if (File.Exists(folderPath))
                {
                    return Path.GetDirectoryName(folderPath);
                }
            }
            return "Assets";
        }
    }

    internal class CreateScriptEndNameEditAction : EndNameEditAction
    {
        public ScriptTemplateData templateData;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            WriteTemplateToFile(pathName, templateData);

            if (templateData.otherItems != null && templateData.otherItems.Count > 0)
            {
                string baseName = Path.GetFileNameWithoutExtension(pathName);

                foreach (var item in templateData.otherItems)
                {
                    string otherPath = pathName.Replace(baseName, baseName + item.suffix);
                    WriteTemplateToFile(otherPath, item);
                    AssetDatabase.LoadAssetAtPath<Object>(otherPath);
                }
            }
            AssetDatabase.Refresh();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            Selection.SetActiveObjectWithContext(obj, obj);
        }

        private void WriteTemplateToFile(string path, ScriptTemplateData templateData)
        {
            string processedText = ReplacePlaceholders(path, templateData);
            File.WriteAllText(path, processedText);
        }

        private string ReplacePlaceholders(string path, ScriptTemplateData templateData)
        {
            FileInfo fileInfo = new FileInfo(path);
            string scriptName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            string text = templateData.template.text;

            text = ReplaceNamespacePlaceholder(path, text);

            foreach (KeywordReplacement replacement in templateData.keywordReplacements)
            {
                text = text.Replace(replacement.Keyword, replacement.GetReplacement(scriptName));
            }
            return text;
        }

        private static string ReplaceNamespacePlaceholder(string path, string text)
        {
            if (!TemplateSettings.instance.addNamespace)
            {
                return text.Replace("#NAMESPACE#", string.Empty);
            }

            string nameSpace = TemplateSettings.instance.GenerateNamespaceName(path);

            text = text.Replace("#NAMESPACE#", $"namespace {nameSpace}{Environment.NewLine}{{");

            int openBraceIndex = text.IndexOf('{') + 1;

            string innerContent = text.Substring(openBraceIndex);
            string indentedContent = "    " + innerContent.Replace(Environment.NewLine, Environment.NewLine + "    ");

            string result = text.Substring(0, openBraceIndex) + indentedContent + Environment.NewLine + "}";

            return result;
        }
    }
}