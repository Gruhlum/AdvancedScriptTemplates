using UnityEditor;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    public class SettingsWindow : EditorWindow
    {
        private UnityEditor.Editor editor;

        private void OnEnable()
        {
            editor = UnityEditor.Editor.CreateEditor(TemplateSettings.instance);
            titleContent = new UnityEngine.GUIContent("Template Settings");
        }

        [MenuItem("Tools/AdvancedScriptTemplates/Settings", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow(typeof(SettingsWindow));
        }
        private void OnGUI()
        {
            editor.OnInspectorGUI();
        }
        private void OnDestroy()
        {
            TemplateSettings.instance.Save();
        }
    }
}