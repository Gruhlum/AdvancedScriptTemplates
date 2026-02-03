using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [Serializable]
    public class NamespaceRegenerator
    {
        [MenuItem("Assets/Regenerate Namespace", priority = 19, secondaryPriority = 1000)]
        public static void RegenerateNamespaces()
        {
            List<string> scriptPaths = FindAllScriptPaths();

            if (scriptPaths.Count == 0)
            {
                Debug.LogWarning("No C# scripts found to regenerate namespaces.");
                return;
            }

            foreach (string path in scriptPaths)
            {
                RegenerateNamespace(path);
            }

            AssetDatabase.Refresh();
        }
        private static void RegenerateNamespace(string path)
        {
            TemplateSettings settings = TemplateSettings.instance;

            if (!settings.addNamespace)
            {
                return;
            }

            string text = File.ReadAllText(path);
            string newNamespace = settings.GenerateNamespaceName(path);

            string updated = ReplaceNamespace(text, newNamespace);

            File.WriteAllText(path, updated);
        }
        private static List<string> FindAllScriptPaths()
        {
            Object[] selected = Selection.GetFiltered<Object>(SelectionMode.Assets);
            List<string> results = new List<string>();

            foreach (Object obj in selected)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (path.EndsWith(".cs"))
                {
                    results.Add(path);
                }
                else if (AssetDatabase.IsValidFolder(path))
                {
                    results.AddRange(FindScriptsInFolder(path));
                }
            }

            return results.Distinct().ToList();
        }
        private static string ReplaceNamespace(string text, string newNamespace)
        {
            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].TrimStart();

                if (trimmed.StartsWith("namespace "))
                {
                    lines[i] = "namespace " + newNamespace;
                    break;
                }
            }

            return string.Join(Environment.NewLine, lines);
        }
        private static IEnumerable<string> FindScriptsInFolder(string folder)
        {
            string[] guids = AssetDatabase.FindAssets("t:Script", new[] { folder });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.EndsWith(".cs"))
                {
                    yield return path;
                }
            }
        }
    }
}