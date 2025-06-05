using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HexTecGames.Basics;
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
            foreach (var scriptTemplateData in TemplateSettings.instance.scriptTemplateDatas)
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

        [MenuItem("Assets/Fix Namespaces", priority = 19, secondaryPriority = 10000)]
        public static void FixNamespaces()
        {
            var folderPaths = GetFolderPaths();

            foreach (var folderPath in folderPaths)
            {
                FixNamespacesForFolder(folderPath);
            }
        }
        private static void FixNamespacesForFolder(string folderPath)
        {
            List<string> scriptPaths = new List<string>();


            Object[] selectedObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);
            var filePaths = Directory.GetFiles(folderPath);

            foreach (var filePath in filePaths)
            {
                if (filePath.EndsWith(".cs"))
                {
                    scriptPaths.Add(filePath.Replace('\\', '/'));
                    Debug.Log(filePath);
                }
            }

            List<Object> scriptObjects = new List<Object>();
            foreach (var path in scriptPaths)
            {
                scriptObjects.Add(AssetDatabase.LoadAssetAtPath<Object>(path));
                //Undo.RecordObject(TemplateSettings.instance, "Namespace Fix");
            }
            Undo.RecordObjects(scriptObjects.ToArray(), "Namespace Fix");

            foreach (var path in scriptPaths)
            {
                FixNamespace(path);
            }

            AssetDatabase.Refresh();
        }

        private static void FixNamespace(string path)
        {
            string scriptText = File.ReadAllText(path);
            int nameSpaceStart = scriptText.IndexOf("namespace");
            int nameSpaceEnd = scriptText.Substring(nameSpaceStart).IndexOf(Environment.NewLine);
            string currentNameSpace = scriptText.Substring(nameSpaceStart, nameSpaceEnd);

            var fixedNameSpace = "namespace " + TemplateSettings.instance.GenerateNamespaceName(path);

            Debug.Log(currentNameSpace + " -> " + fixedNameSpace);

            scriptText = scriptText.Replace(currentNameSpace, fixedNameSpace);

            File.WriteAllText(path, scriptText);
        }

        public static void CreateTemplate(ScriptTemplateData data)
        {
            CreateScriptEndNameEditAction create = ScriptableObject.CreateInstance<CreateScriptEndNameEditAction>();
            create.template = data.template;
            create.data = data;
            string newPath = Path.Combine(GetFolderPath(), data.newFileName + ".cs");
            Texture2D icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, newPath, icon, null);
        }

        private static List<string> GetFolderPaths()
        {
            Object[] selectedObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);

            if (selectedObjects == null)
            {
                return null;
            }
            if (selectedObjects.Length == 0)
            {
                return null;
            }

            List<string> folderPaths = new List<string>();

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                string folderPath = AssetDatabase.GetAssetPath(selectedObjects[i]);
                if (AssetDatabase.IsValidFolder(folderPath))
                {
                    folderPaths.Add(folderPath);
                }

            }
            return folderPaths;
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
        public TextAsset template;
        public ScriptTemplateData data;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string templateText = ReplacePlaceholders(pathName, template);
            File.WriteAllText(pathName, templateText);
            if (data.otherItems != null && data.otherItems.Count > 0)
            {
                FileInfo fileInfo = new FileInfo(pathName);
                string nameOfScript = Path.GetFileNameWithoutExtension(fileInfo.Name);

                foreach (var otherData in data.otherItems)
                {
                    string otherPathName = pathName.Replace(nameOfScript, nameOfScript + otherData.suffix);
                    Debug.Log(otherPathName);
                    templateText = ReplacePlaceholders(otherPathName, otherData.template);
                    File.WriteAllText(otherPathName, templateText);
                    AssetDatabase.LoadAssetAtPath<Object>(otherPathName);
                }
            }
            AssetDatabase.Refresh();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            Selection.SetActiveObjectWithContext(obj, obj);
        }

        private string ReplacePlaceholders(string path, TextAsset template)
        {
            FileInfo fileInfo = new FileInfo(path);
            string nameOfScript = Path.GetFileNameWithoutExtension(fileInfo.Name);

            string text = template.text;

            string nameSpace = TemplateSettings.instance.GenerateNamespaceName(path);

            if (TemplateSettings.instance.addNameSpace)
            {
                int spaceIndex = text.IndexOf("#NAMESPACE#");
                text = text.Insert(spaceIndex + "#NAMESPACE#".Length, Environment.NewLine + "{");

                int firstBracketIndex = text.IndexOf("{") + 1;

                string classText = text.Substring(firstBracketIndex, text.Length - firstBracketIndex);
                classText = classText.Replace(Environment.NewLine, Environment.NewLine + "    ");

                text = text.Remove(firstBracketIndex);
                text = text.Insert(firstBracketIndex, classText);
                text = text.Replace("#NAMESPACE#", "namespace " + nameSpace);
                text = text.Insert(text.Length, Environment.NewLine + "}");
            }
            else text = text.Replace("#NAMESPACE#", string.Empty);

            text = text.Replace("#SCRIPTNAME#", nameOfScript);
            text = text.Replace("#SCRIPTNAMEWITHOUTEDITOR#", nameOfScript.Replace("Editor", string.Empty));
            text = text.Replace("#SCRIPTNAMEWITHOUTDISPLAY#", nameOfScript.Replace("Display", string.Empty));
            text = text.Replace("#SCRIPTNAMEWITHOUTCONTROLLER#", nameOfScript.Replace("Controller", string.Empty));
            text = text.Replace("#SCRIPTNAMEWITHOUTDISPLAYANDCONTROLLER#", nameOfScript.Replace("Controller", string.Empty).Replace("Display", string.Empty));
            text = text.Replace("#SCRIPTNAMEWITHOUTDISPLAYLOWER#", nameOfScript.Replace("Display", string.Empty).ToLowerInvariant());
            text = text.Replace("#SCRIPTABLEOBJECTNAME#", TemplateSettings.instance.GetScriptableObjectName());
            text = text.Replace("#COMPANYNAME#", Application.companyName);
            text = text.Replace("#PROJECTNAME#", TemplateSettings.instance.GetProjectName(path));
            text = text.Replace("#NAMESPACE#", "namespace");
            text = text.Replace("#NAMESPACENAME#", nameSpace);
            return text;
        }
    }
}