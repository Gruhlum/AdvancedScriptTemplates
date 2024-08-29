using HexTecGames.Basics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    //[CreateAssetMenu(menuName = "HexTecGames/PackageTest/TemplateSettings")]
    public class TemplateSettings : ScriptableObject
    {
        private static TemplateSettings s_Instance;

        public static TemplateSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<TemplateSettings>("Template Settings");
                }

                return s_Instance;
            }
        }

        public enum DefaultNameSpaceType { custom, companyName }

        [TextArea]
        public string templatePath;

        [Header("Namespace Settings")]
        public bool addNameSpace = true;
        public bool addDefaultNameSpace = true;
        public DefaultNameSpaceType defaultNameSpaceType = DefaultNameSpaceType.companyName;
        public string customNameSpace;
        public bool addFolderNameSpace = true;

        public List<string> ignoreFolders = new List<string>() { "Assets" };


        private void Awake()
        {
            TryToSetTemplatePath();
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

            if (folderPath.Contains("Packages/com"))
            {
                return GetDefaultNameSpace();
            }

            foreach (var ignoreWord in ignoreFolders)
            {
                folderPath = folderPath.Replace($"{ignoreWord}/", string.Empty);
            }

            int indexOfClassName = folderPath.LastIndexOf('/');
            if (indexOfClassName == -1)
            {
                return GetDefaultNameSpace();             
            }

            folderPath = folderPath.Remove(indexOfClassName, folderPath.Length - indexOfClassName);
            folderPath = folderPath.Replace('/', '.').Replace(" ", string.Empty);
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
        private void TryToSetTemplatePath()
        {
            if (!string.IsNullOrEmpty(templatePath))
            {
                return;
            }

            string path = AssetDatabase.GetAssetPath(this);

            path = Directory.GetParent(path).Parent.FullName;
            path = Path.GetRelativePath("Assets", path);
            templatePath = path;
        }
    }
}