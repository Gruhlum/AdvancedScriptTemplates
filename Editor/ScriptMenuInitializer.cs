using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class ScriptMenuInitializer
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.delayCall -= CreateMenus;
            EditorApplication.delayCall += CreateMenus;
        }

        private static void CreateMenus()
        {
            TemplateSettings settings = TemplateSettings.instance;

            if (settings.scriptTemplateDatas == null)
            {
                return;
            }

            foreach (ScriptTemplateData data in settings.scriptTemplateDatas)
            {
                data.VerifyMenu();
            }
        }
    }
}