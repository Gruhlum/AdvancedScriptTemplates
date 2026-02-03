using UnityEditor;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    public static class FolderSelector
    {
        private static Texture Texture
        {
            get
            {
                if (texture == null)
                {
                    texture = AssetDatabase.LoadAssetAtPath<Texture>(IconPath);
                }
                return texture;
            }
        }
        private static Texture texture = default;

        private const string IconPath = "Packages/com.hextecgames.advancedscripttemplates/Icons/Folder_Normal.png";

        [InitializeOnLoadMethod]
        private static void Start()
        {
            EditorApplication.projectWindowItemOnGUI -= DrawFolderIcon;
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
        }

        [MenuItem("Tools/AdvancedScriptTemplates/Toggle Folder %n")]
        private static void ToggleFolder()
        {
            if (Selection.activeObject == null)
            {
                return;
            }
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
            var settings = TemplateSettings.instance;
            if (settings.selectedPaths != null && !settings.selectedPaths.Contains(path))
            {
                return;
            }
            Rect imageRect = GetIconRect(rect);

            GUI.DrawTexture(imageRect, Texture);
        }

        private static Rect GetIconRect(Rect rect)
        {
            if (rect.height > 20)
            {
                return new Rect(rect.x, rect.y, rect.width, rect.width);
            }

            if (rect.x > 20)
            {
                return new Rect(rect.x, rect.y, rect.height, rect.height);
            }

            return new Rect(rect.x + 3, rect.y, rect.height, rect.height);
        }

    }
}