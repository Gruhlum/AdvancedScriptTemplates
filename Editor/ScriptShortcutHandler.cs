using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class ScriptShortcutHandler
    {
        [Shortcut("HexTec/Create Script 1", KeyCode.Alpha1, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        private static void Shortcut1()
        {
            ScriptTemplateCreator.CreateTemplateByIndex(0);
        }

        [Shortcut("HexTec/Create Script 2", KeyCode.Alpha2, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        private static void Shortcut2()
        {
            ScriptTemplateCreator.CreateTemplateByIndex(1);
        }

        [Shortcut("HexTec/Create Script 3", KeyCode.Alpha3, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        private static void Shortcut3()
        {
            ScriptTemplateCreator.CreateTemplateByIndex(2);
        }

        [Shortcut("HexTec/Create Script 4", KeyCode.Alpha4, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        private static void Shortcut4()
        {
            ScriptTemplateCreator.CreateTemplateByIndex(3);
        }

        [Shortcut("HexTec/Create Script 5", KeyCode.Alpha5, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        private static void Shortcut5()
        {
            ScriptTemplateCreator.CreateTemplateByIndex(4);
        }

        [Shortcut("HexTec/Create Script 6", KeyCode.Alpha6, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        private static void Shortcut6()
        {
            ScriptTemplateCreator.CreateTemplateByIndex(5);
        }
    }
}