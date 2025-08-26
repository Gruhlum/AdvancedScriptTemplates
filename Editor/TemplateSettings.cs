using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HexTecGames.Basics;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [FilePath("Assets/Plugins/AdvancedScriptTemplates/Settings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class TemplateSettings : ScriptableSingleton<TemplateSettings>
    {
        public enum DefaultNamespaceType { custom, companyName }

        public List<ScriptTemplateData> scriptTemplateDatas;
        [SerializeField] private KeywordReplacementCollection replacementData = default;

        [Header("Namespace Settings")]
        public bool addNamespace = true;
        public bool addDefaultNamespace = true;
        public DefaultNamespaceType defaultNamespaceType = DefaultNamespaceType.companyName;
        [DrawIf(nameof(defaultNamespaceType), DefaultNamespaceType.custom)]
        public string customNamespace;

        public DefaultNamespaceType defaultNameSO = DefaultNamespaceType.companyName;
        [DrawIf(nameof(defaultNameSO), DefaultNamespaceType.custom)]
        [Tooltip("The first sub menu for creating the SO")] public string customScriptableObjectName;

        [TextArea] public string ignoreFolders = "Assets, Scripts, Game, Test, Runtime";

        public List<string> selectedPaths;

        public KeywordReplacementCollection ReplacementData
        {
            get
            {
                return this.replacementData;
            }
            set
            {
                this.replacementData = value;
            }
        }

        private List<string> GenerateIgnoredFolders()
        {
            string result = ignoreFolders;
            result = result.RemoveSpaces();
            return result.Split(",").ToList();
        }


        private string GetPackageName(string folderPath)
        {
            int startIndex = folderPath.IndexOf("com.");
            int firstSlash = folderPath.Substring(startIndex).IndexOf('/');
            return folderPath.Substring(startIndex, firstSlash);
        }

        public string GenerateNamespaceName(string folderPath) // Assets/Test/IJohn.cs
        {
            List<string> removeWords = new List<string>();

            if (selectedPaths != null)
            {
                foreach (string selectedPath in selectedPaths)
                {
                    if (folderPath.Contains(selectedPath))
                    {
                        //Package Selected: /com.hextecgames.advancedscripttemplates/Editor
                        //Normal Selected: Assets/Scripts/Tier1/Tier2
                        //Path:      /Scripts
                        DirectoryInfo directoryInfo = new DirectoryInfo(selectedPath);
                        //Debug.Log(directoryInfo.Name);
                        removeWords.Add("/" + directoryInfo.Name + "/");
                    }
                }
            }

            foreach (string word in removeWords)
            {
                folderPath = folderPath.Replace(word, "/");
            }

            if (folderPath.Contains("Packages/com"))
            {
                return GetPackageNameSpace(folderPath);
            }
            else return TurnPathIntoNamespace(folderPath);
        }

        private PackageInfo GetPackageInfo(string packageName)
        {
            PackageInfo[] packages = PackageInfo.GetAllRegisteredPackages();
            foreach (PackageInfo package in packages)
            {
                //Debug.Log(package.name);
                if (package.name == packageName)
                {
                    return package;
                }
            }
            return null;
        }

        private string GetPackageNameSpace(string folderPath)
        {
            // Packages/com.unity.toolchain.win-x86_64-linux-x86_64
            string packageName = GetPackageName(folderPath);
            PackageInfo packageInfo = GetPackageInfo(packageName);

            string cleanPath = folderPath; // Packages/com.unity.toolchain.win-x86_64-linux-x86_64/Editor/TestSc.cs
            cleanPath = cleanPath.Replace("Packages/", string.Empty); // com.unity.toolchain.win-x86_64-linux-x86_64/Editor/TestSc.cs
            cleanPath = cleanPath.Replace(packageName, packageInfo.displayName);
            return TurnPathIntoNamespace(cleanPath);
        }

        private string TurnPathIntoNamespace(string path)
        {
            int lastSlashIndex = path.LastIndexOf('/');
            if (lastSlashIndex == -1)
            {
                return GetDefaultNameSpace();
            }

            path = path.Substring(0, lastSlashIndex);

            foreach (string ignore in GenerateIgnoredFolders())
            {
                // Match folder names exactly, surrounded by slashes or start/end of string
                string pattern = $@"(?<=^|/){Regex.Escape(ignore)}(?=/|$)";
                path = Regex.Replace(path, pattern, string.Empty);

                // Clean up any double slashes or leading/trailing slashes
                path = Regex.Replace(path, @"//+", "/").Trim('/');
            }

            path = path.Replace('/', '.').RemoveSpaces();

            return string.IsNullOrEmpty(path) ? GetDefaultNameSpace() : $"{GetDefaultNameSpace()}.{path}";
        }

        private string GetDefaultNameSpace()
        {
            string namespaceName = null;
            if (addDefaultNamespace)
            {
                if (defaultNamespaceType == DefaultNamespaceType.custom)
                {
                    namespaceName = customNamespace.Replace(" ", string.Empty);
                }
                else namespaceName = Application.companyName.Replace(" ", string.Empty);
            }
            return namespaceName;
        }
        public string GetScriptableObjectName()
        {
            if (defaultNameSO == DefaultNamespaceType.companyName)
            {
                return Application.companyName;
            }
            else return customScriptableObjectName;
        }

        private bool PathIsPackage(string path)
        {
            return path.Contains("com.");
        }

        public string GetProjectName(string path)
        {
            if (PathIsPackage(path))
            {
                string packageName = GetPackageName(path);
                PackageInfo packageInfo = GetPackageInfo(packageName);
                return packageInfo.displayName;
            }
            else return Application.productName;
        }

        public void TogglePath(string path)
        {
            selectedPaths ??= new List<string>();
            if (selectedPaths.Contains(path))
            {
                selectedPaths.Remove(path);
            }
            else selectedPaths.Add(path);

            Save();
        }

        public void RemoveUnusedPaths()
        {
            for (int i = selectedPaths.Count - 1; i >= 0; i--)
            {
                string path = selectedPaths[i];
                if (!AssetDatabase.IsValidFolder(path))
                {
                    selectedPaths.RemoveAt(i);
                    Debug.Log($"Removing {path}");
                }
            }
        }

        public void Save()
        {
            string path = Path.Combine(Application.dataPath, "Plugins", "AdvancedScriptTemplates");
            if (Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Save(false);
        }
    }
}