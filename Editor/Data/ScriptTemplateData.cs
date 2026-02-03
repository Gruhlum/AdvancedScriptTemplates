using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [CreateAssetMenu(menuName = "HexTecGames/AdvancedScriptTemplates/ScriptTemplateData")]
    public class ScriptTemplateData : ScriptableObject
    {
        public string menuItemName;
        public string newFileName;
        public string suffix;

        public int priority = 10;
        public TextAsset template;
        public KeywordReplacementCollection keywordReplacements;
        public List<ScriptTemplateData> otherItems = new List<ScriptTemplateData>();

        private const string MENU_PATH = "Assets/Create";

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(menuItemName))
            {
                menuItemName = name;
            }
            if (string.IsNullOrEmpty(newFileName))
            {
                newFileName = name;
            }
        }

        public string GetFullPath()
        {
            return $"{MENU_PATH}/{menuItemName}";
        }

        public void VerifyMenu()
        {
            string path = GetFullPath();
            bool exists = _Menu.MenuItemExists(path);
            if (!exists)
            {
                _Menu.AddMenuItem(path, string.Empty, false, priority, MenuClicked, () => true);
            }
        }
        private void MenuClicked()
        {
            if (template == null)
            {
                Debug.LogError($"Template for '{name}' is missing. Cannot create script.");
                return;
            }
            ScriptTemplateCreator.CreateTemplate(this);
        }
    }
}