using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    public class SettingsWindow : EditorWindow
    {
        [SerializeField] private TemplateSettings settings = default;
        [MenuItem("Tools/Settings/Advanced ScriptTemplates")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SettingsWindow));
        }
        private void OnGUI()
        {
            UnityEditor.Editor m_MyScriptableObjectEditor = UnityEditor.Editor.CreateEditor(settings);
            m_MyScriptableObjectEditor.OnInspectorGUI();
        }
    }
}