using HexTecGames.Basics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [FilePath("Assets/Plugins/AdvancedScriptTemplates/Settings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class TemplateSettings : ScriptableSingleton<TemplateSettings>
    {
        public enum DefaultNameSpaceType { custom, companyName }
        public List<ScriptTemplateData> scriptTemplateDatas;
        public List<KeywordReplacement> keywordReplacements;
        //public KeywordReplacement replacement;
        [Header("Namespace Settings")]
        public bool addNameSpace = true;
        public bool addDefaultNameSpace = true;
        [Space]
        public DefaultNameSpaceType defaultNameSpaceType = DefaultNameSpaceType.companyName;
        [DrawIf(nameof(defaultNameSpaceType), DefaultNameSpaceType.custom)]
        public string customNameSpace;

        public DefaultNameSpaceType defaultNameSO = DefaultNameSpaceType.companyName;
        [DrawIf(nameof(defaultNameSO), DefaultNameSpaceType.custom)]
        [Tooltip("The first sub-menu for creating the SO")] public string customScriptableObjectName;

        public bool addFolderNameSpace = true;

        [TextArea] public string ignoreFolders = "Assets, Scripts, Game, Test, Runtime";

        public List<string> selectedPaths;


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
            if (!addFolderNameSpace)
            {
                if (addDefaultNameSpace)
                {
                    return GetDefaultNameSpace();
                }
            }
            List<string> removeWords = new List<string>();

            if (selectedPaths != null)
            {
                foreach (var selectedPath in selectedPaths)
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

            foreach (var word in removeWords)
            {
                folderPath = folderPath.Replace(word, "/");
            }

            if (folderPath.Contains("Packages/com"))
            {
                return GetPackageNameSpace(folderPath);
            }
            else return TurnPathIntoNamespace(folderPath);
        }

        private string GetPackageNameSpace(string folderPath)
        {
            // Packages/com.unity.toolchain.win-x86_64-linux-x86_64
            string packageName = GetPackageName(folderPath);
            //Debug.Log(packageName);
            var packages = PackageInfo.GetAllRegisteredPackages();
            string packageDisplayName = packageName;
            foreach (var package in packages)
            {
                //Debug.Log(package.name);
                if (package.name == packageName)
                {
                    packageDisplayName = package.displayName;
                    break;
                }
            }

            string cleanPath = folderPath; // Packages/com.unity.toolchain.win-x86_64-linux-x86_64/Editor/TestSc.cs
            cleanPath = cleanPath.Replace("Packages/", string.Empty); // com.unity.toolchain.win-x86_64-linux-x86_64/Editor/TestSc.cs
            cleanPath = cleanPath.Replace(packageName, packageDisplayName);
            return TurnPathIntoNamespace(cleanPath);
        }

        private string TurnPathIntoNamespace(string path)
        {
            int indexOfClassName = path.LastIndexOf('/');
            if (indexOfClassName == -1)
            {
                return GetDefaultNameSpace();
            }

            path = path.Remove(indexOfClassName, path.Length - indexOfClassName);
            path = path.Replace('/', '.');

            foreach (var ignoreWord in GenerateIgnoredFolders())
            {
                //Debug.Log(path + " " + ignoreWord);
                if (path.Contains($".{ignoreWord}"))
                {
                    path = path.Replace($".{ignoreWord}", string.Empty);
                }
                else if (path.Contains($"{ignoreWord}."))
                {
                    path = path.Replace($"{ignoreWord}.", string.Empty);
                }
                else path = path.Replace($"{ignoreWord}", string.Empty);
            }

            path = path.RemoveSpaces();
            if (path != string.Empty)
            {
                return string.Join('.', GetDefaultNameSpace(), path);
            }
            else return GetDefaultNameSpace();
        }

        private string GetDefaultNameSpace()
        {
            string namespaceName = null;
            if (addDefaultNameSpace)
            {
                if (defaultNameSpaceType == DefaultNameSpaceType.custom)
                {
                    namespaceName = customNameSpace.Replace(" ", string.Empty);
                }
                else namespaceName = Application.companyName.Replace(" ", string.Empty);
            }
            return namespaceName;
        }
        public string GetScriptableObjectName()
        {
            if (defaultNameSO == DefaultNameSpaceType.companyName)
            {
                return Application.companyName;
            }
            else return customScriptableObjectName;
        }

        public void TogglePath(string path)
        {
            if (selectedPaths == null)
            {
                selectedPaths = new List<string>();
            }
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
                var path = selectedPaths[i];
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