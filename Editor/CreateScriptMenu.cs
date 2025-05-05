using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.ShortcutManagement;
using UnityEngine;

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

        public static void CreateTemplate(ScriptTemplateData data)
        {
            CreateScriptEndNameEditAction create = ScriptableObject.CreateInstance<CreateScriptEndNameEditAction>();
            create.template = data.template;
            create.data = data;
            string newPath = Path.Combine(GetFolder(), data.newFileName + ".cs");
            Texture2D icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, newPath, icon, null);
        }
        private static string GetFolder()
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

        private string ReplacePlaceholders(string pathName, TextAsset template)
        {
            FileInfo fileInfo = new FileInfo(pathName);
            string nameOfScript = Path.GetFileNameWithoutExtension(fileInfo.Name);

            string text = template.text;

            if (TemplateSettings.instance.addNameSpace)
            {
                int spaceIndex = text.IndexOf("#NAMESPACE#");
                text = text.Insert(spaceIndex + "#NAMESPACE#".Length, System.Environment.NewLine + "{");

                int firstBracketIndex = text.IndexOf("{") + 1;

                string classText = text.Substring(firstBracketIndex, text.Length - firstBracketIndex);
                classText = classText.Replace(System.Environment.NewLine, System.Environment.NewLine + "    ");

                text = text.Remove(firstBracketIndex);
                text = text.Insert(firstBracketIndex, classText);
                text = text.Replace("#NAMESPACE#", "namespace " + TemplateSettings.instance.GenerateNamespaceName(pathName));
                text = text.Insert(text.Length, System.Environment.NewLine + "}");
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
            text = text.Replace("#PROJECTNAME#", Application.productName);
            text = text.Replace("#NAMESPACE#", "namespace");
            text = text.Replace("#NAMESPACENAME#", TemplateSettings.instance.GenerateNamespaceName(pathName));
            return text;
        }
    }
}