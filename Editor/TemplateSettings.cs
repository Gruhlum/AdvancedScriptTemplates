using HexTecGames.Basics;
using System.Collections;
using System.Collections.Generic;
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

        public bool addNameSpace;
        [DrawIf(nameof(addNameSpace), true)]public bool addDefaultNamespace;

		[DrawIf(nameof(addDefaultNamespace), true)] public DefaultNameSpaceType defaultNameSpaceType = DefaultNameSpaceType.companyName;
        [DrawIf(nameof(defaultNameSpaceType), DefaultNameSpaceType.custom), DrawIf(nameof(addDefaultNamespace), true), DrawIf(nameof(addNameSpace), true)] 
        public string customNameSpace;

        public List<string> ignoreWords = new List<string>() { "Assets" };

        public string GenerateNamespaceName(string folderPath) // Assets/Test/IJohn.cs
        {
            foreach (var ignoreWord in ignoreWords)
            {
                folderPath = folderPath.Replace($"{ignoreWord}/", string.Empty);
            }

            int indexOfClassName = folderPath.LastIndexOf('/');
            folderPath = folderPath.Remove(indexOfClassName, folderPath.Length - indexOfClassName);

            folderPath = folderPath.Replace('/', '.');

            Debug.Log(folderPath);
            string namespaceName = null;
            if (addDefaultNamespace)
            {
                if (defaultNameSpaceType == DefaultNameSpaceType.custom)
                {
                    namespaceName = customNameSpace;
                }
                else namespaceName = Application.companyName;
            }
            return string.Join('.', namespaceName, folderPath);
        }
	}
}