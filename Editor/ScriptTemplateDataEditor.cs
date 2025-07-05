using UnityEditor;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [CustomEditor(typeof(ScriptTemplateData))]
    public class ScriptTemplateDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(6);
            if (GUILayout.Button("Refresh Editor", GUILayout.Height(24)))
            {
                EditorUtility.RequestScriptReload();
            }
        }
    }
}