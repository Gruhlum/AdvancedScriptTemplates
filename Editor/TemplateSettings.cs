using HexTecGames.Basics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [FilePath("Assets/Plugins/AdvancedScriptTemplates/Settings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class TemplateSettings : ScriptableSingleton<TemplateSettings>
    {
        public enum DefaultNameSpaceType { custom, companyName }
        public List<ScriptTemplateData> scriptTemplateDatas;
        public List<KeywordReplacement> keywordReplacements;
        public KeywordReplacement replacement;
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

        [TextArea] public string ignoreFolders = "Assets, Scripts, Game, Test, com.package";


        [SerializeField] public List<string> selectedPaths = new List<string>();


        private List<string> GenerateIgnoredFolders()
        {
            string result = ignoreFolders;
            result = result.RemoveSpaces();
            return result.Split(",").ToList();
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

            foreach (var selectedPath in selectedPaths)
            {
                Debug.Log(folderPath + " - " + selectedPath);
                if (folderPath.Contains(selectedPath))
                {
                    //Selected: Assets/Scripts/Tier1/Tier2
                    //Path:      /Scripts
                    DirectoryInfo directoryInfo = new DirectoryInfo(selectedPath);
                    Debug.Log(directoryInfo.Name);
                    removeWords.Add("/" + directoryInfo.Name);
                }
            }

            foreach (var word in removeWords)
            {
                folderPath = folderPath.Replace(word, string.Empty);
            }

            if (folderPath.Contains("Packages/com"))
            {
                return GetDefaultNameSpace();
            }

            folderPath = folderPath.RemoveSpaces();

            foreach (var ignoreWord in GenerateIgnoredFolders())
            {
                folderPath = folderPath.Replace($"{ignoreWord}/", string.Empty);
            }

            int indexOfClassName = folderPath.LastIndexOf('/');
            if (indexOfClassName == -1)
            {
                return GetDefaultNameSpace();
            }

            Debug.Log(selectedPaths.Count + " - " + folderPath);



            folderPath = folderPath.Remove(indexOfClassName, folderPath.Length - indexOfClassName);
            folderPath = folderPath.Replace('/', '.');
            string namespaceName = GetDefaultNameSpace();
            return string.Join('.', namespaceName, folderPath);
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
            if (selectedPaths.Contains(path))
            {
                selectedPaths.Remove(path);
            }
            else selectedPaths.Add(path);

            Save();
        }

        public void Save()
        {
            string path = Path.Combine(Application.dataPath, "Plugins", "AdvancedScriptTemplates");
            if (Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Save(true);
        }
    }
}