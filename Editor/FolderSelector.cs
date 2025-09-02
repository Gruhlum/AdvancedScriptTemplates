using UnityEditor;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    public class FolderSelector
    {
        [SerializeField] private static Texture texture = default;


        [InitializeOnLoadMethod]
        private static void Start()
        {
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
        }

        [MenuItem("Tools/AdvancedScriptTemplates/Toggle Folder %n")]
        private static void ToggleFolder()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!AssetDatabase.IsValidFolder(path))
            {
                return;
            }
            TemplateSettings.instance.TogglePath(path);
            EditorApplication.RepaintProjectWindow();
        }
        [MenuItem("Tools/AdvancedScriptTemplates/Remove unused Folders")]
        public static void RemoveUnusedFolderPaths()
        {
            TemplateSettings.instance.RemoveUnusedPaths();
        }

        private static void DrawFolderIcon(string guid, Rect rect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path == string.Empty)
            {
                return;
            }
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (!AssetDatabase.IsValidFolder(path))
            {
                return;
            }
            if (TemplateSettings.instance.selectedPaths != null && !TemplateSettings.instance.selectedPaths.Contains(path))
            {
                return;
            }
            Rect imageRect;

            if (rect.height > 20)
            {
                imageRect = new Rect(rect.x, rect.y, rect.width, rect.width);
            }
            else if (rect.x > 20)
            {
                imageRect = new Rect(rect.x, rect.y, rect.height, rect.height);
            }
            else
            {
                imageRect = new Rect(rect.x + 3, rect.y, rect.height, rect.height);
            }

            if (texture == null)
            {
                texture = Resources.Load<Texture>("Folder_Normal");
            }
            GUI.DrawTexture(imageRect, texture);
        }
    }
}