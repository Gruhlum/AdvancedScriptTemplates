using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [CreateAssetMenu(menuName = "HexTecGames/AdvancedScriptTemplates/ScriptTemplateData")]
    public class ScriptTemplateData : ScriptableObject
    {
        public string menuItemName;
        public string newFileName;
        //[Tooltip("% = CTRL | # = SHIFT | & = ALT")] public string shortcut;
        public int priority = 10;
        public TextAsset template;
        public List<TemplateGroupItem> otherItems = new List<TemplateGroupItem>();

        private const string MENU_PATH = "Assets/Create";
        private static bool MENU_SHOULD_EXIST = true;


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
            bool menuExist = _Menu.MenuItemExists(path);
            bool menuShouldExist = MENU_SHOULD_EXIST;

            // Menu is removed but should be added
            if (!menuExist && menuShouldExist)
            {
                //TODO: Shortcut doesn't work!
                _Menu.AddMenuItem(path, string.Empty, false, priority, MenuClicked, () => true);
            }
            // Menu exist but should be removed
            else if (menuExist && !menuShouldExist)
            {
                _Menu.RemoveMenuItem(path);
            }
        }
        private void MenuClicked()
        {
            CreateScriptMenu.CreateTemplate(this);
        }
    }
}