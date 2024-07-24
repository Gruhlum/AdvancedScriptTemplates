using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    public static class CreateScriptMenu
    {
        //public TemplateSettings settings;

        static string templateFolder = "Plugins/ScriptTemplates";

        [MenuItem("Assets/Create/MonoBehaviour", priority = 1, validate = false)]
        static void CreateMonoBehaviourMenuItem()
        {
            string pathToTemplate = Path.Combine(Application.dataPath, templateFolder, "MonoBehaviourTemplate.txt");
            CreateTemplate(pathToTemplate, "MonoBehaviour");
        }
        [MenuItem("Assets/Create/ScriptableObject", priority = 2)]
        static void CreateScriptableObjectMenuItem()
        {
            string pathToTemplate = Path.Combine(Application.dataPath, templateFolder, "ScriptableObjectTemplate.txt");
            CreateTemplate(pathToTemplate, "ScriptableObject");
        }
        [MenuItem("Assets/Create/SerializedClass", priority = 3)]
        static void CreateSerializedClassMenuItem()
        {
            string pathToTemplate = Path.Combine(Application.dataPath, templateFolder, "SerializedClassTemplate.txt");
            CreateTemplate(pathToTemplate, "SerializedClass");
        }
        [MenuItem("Assets/Create/Interface", priority = 4)]
        static void CreateInterfaceMenuItem()
        {
            string pathToTemplate = Path.Combine(Application.dataPath, templateFolder, "InterfaceTemplate.txt");

            CreateTemplate(pathToTemplate, "IInterface");
        }
        [MenuItem("Assets/Create/Editor", priority = 5)]
        static void CreateEditorMenuItem()
        {
            string pathToTemplate = Path.Combine(Application.dataPath, templateFolder, "EditorTemplate.txt");

            CreateTemplate(pathToTemplate, "SomeEditor");
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

        static void CreateTemplate(string templatePath, string defaultName)
        {
            CreateScriptEndNameEditAction create = ScriptableObject.CreateInstance<CreateScriptEndNameEditAction>();
            create.templatePath = templatePath;
            string newPath = Path.Combine(GetFolder(), defaultName + ".cs");
            Texture2D icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, newPath, icon, null);
        }
    }

    internal class CreateScriptEndNameEditAction : EndNameEditAction
    {
        public string templatePath;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Debug.Log(pathName);
            ReplacePlaceholders(pathName);
            AssetDatabase.Refresh();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            Selection.SetActiveObjectWithContext(obj, obj);
        }

        /*
         * Find place for namespace -> after last using + one more line down (maybe a check if there is no empty line
         * Place the namespace name
         * add an opening bracket below
         * add four spaces to every other line including the last '}'
         * add a closing bracket below
         * 
         * 
         * 
         * 
         */

        private void ReplacePlaceholders(string pathName)
        {
            FileInfo fileInfo = new FileInfo(pathName);
            string nameOfScript = Path.GetFileNameWithoutExtension(fileInfo.Name);

            string text = File.ReadAllText(templatePath);

            if (TemplateSettings.Instance.addNameSpace)
            {
                int spaceIndex = text.IndexOf("#NAMESPACE#");
                text = text.Insert(spaceIndex + "#NAMESPACE#".Length, System.Environment.NewLine + "{");


                int firstBracketIndex = text.IndexOf("{") + 1;
                //int lastBracketIndex = text.LastIndexOf('}');

                string classText = text.Substring(firstBracketIndex, text.Length - firstBracketIndex);
                classText = classText.Replace(System.Environment.NewLine, System.Environment.NewLine + "    ");

                text = text.Remove(firstBracketIndex);
                text = text.Insert(firstBracketIndex, classText);

                //text = text.Insert(text.Length - 1, "}");

                text = text.Replace("#NAMESPACE#", "namespace " + TemplateSettings.Instance.GenerateNamespaceName(pathName));
                text = text.Insert(text.Length, System.Environment.NewLine + "}");
            }
            else text = text.Replace("#NAMESPACE#", string.Empty);

            text = text.Replace("#SCRIPTNAME#", nameOfScript);
            text = text.Replace("#SCRIPTNAMEWITHOUTEDITOR#", nameOfScript.Replace("Editor", string.Empty));
            text = text.Replace("#COMPANYNAME#", Application.companyName);
            text = text.Replace("#PROJECTNAME#", Application.productName);
            text = text.Replace("#BRACKETOPEN#", "{");
            text = text.Replace("#BRACKETCLOSE#", "}");
            text = text.Replace("#NAMESPACE#", "namespace");
            text = text.Replace("#NAMESPACENAME#", TemplateSettings.Instance.GenerateNamespaceName(pathName));

            Debug.Log(pathName);

            File.WriteAllText(pathName, text);
        }
    }
}