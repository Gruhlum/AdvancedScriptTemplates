using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class ScriptNameReplacement : ReplacementFunction
    {
        [SerializeField] private string additionalRemoval = default;
        [SerializeField] private string suffix = default;
        [SerializeField] private bool toLower = default;
        public override string GetReplacement(string scriptName, string path)
        {
            string result = RemoveAdditionalString(scriptName);
            if (!string.IsNullOrEmpty(suffix))
            {
                result += suffix;
            }
            if (toLower)
            {
                return result.ToLowerInvariant();
            }
            else return result;
        }

        private string RemoveAdditionalString(string scriptName)
        {
            if (string.IsNullOrEmpty(additionalRemoval))
            {
                return scriptName;
            }
            else return scriptName.Replace(additionalRemoval, string.Empty);
        }
    }
}