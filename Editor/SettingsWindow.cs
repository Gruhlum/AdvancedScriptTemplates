using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    public class SettingsWindow : EditorWindow
    {
        [MenuItem("Tools/AdvancedScriptTemplates/Settings")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SettingsWindow));
        }
        private void OnGUI()
        {
            UnityEditor.Editor m_MyScriptableObjectEditor = UnityEditor.Editor.CreateEditor(TemplateSettings.instance);
            m_MyScriptableObjectEditor.OnInspectorGUI();
            TemplateSettings.instance.Save();
        }
    }
}