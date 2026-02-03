using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class ScriptTemplateCreator
    {
        private const string ScriptIconName = "cs Script Icon";

        public static void CreateTemplateByIndex(int index)
        {
            TemplateSettings settings = TemplateSettings.instance;

            if (settings.scriptTemplateDatas == null)
            {
                return;
            }

            if (index < 0 || index >= settings.scriptTemplateDatas.Count)
            {
                return;
            }

            CreateTemplate(settings.scriptTemplateDatas[index]);
        }
        public static void CreateTemplate(ScriptTemplateData templateData)
        {
            if (templateData == null)
            {
                Debug.LogError("TemplateData is null.");
                return;
            }

            CreateScriptEndNameEditAction action = ScriptableObject.CreateInstance<CreateScriptEndNameEditAction>();
            action.templateData = templateData;

            string folder = GetFolderPath();
            string newPath = Path.Combine(folder, templateData.newFileName + ".cs");

            Texture2D icon = EditorGUIUtility.IconContent(ScriptIconName).image as Texture2D;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, newPath, icon, null);
        }
        private static string GetFolderPath()
        {
            Object[] selected = Selection.GetFiltered<Object>(SelectionMode.Assets);

            if (selected != null && selected.Length > 0)
            {
                string path = AssetDatabase.GetAssetPath(selected[0]);

                if (AssetDatabase.IsValidFolder(path))
                {
                    return path;
                }

                if (File.Exists(path))
                {
                    return Path.GetDirectoryName(path).Replace('\\', '/');
                }
            }
            return "Assets";
        }
    }
}