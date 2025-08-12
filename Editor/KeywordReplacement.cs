using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class KeywordReplacement
    {
        [SerializeReference] private string keyword = "#KEYWORD#";
        [SubclassSelector, SerializeReference] private ReplacementFunction replacementFunction;

        public string Keyword
        {
            get
            {
                return this.keyword;
            }
            set
            {
                this.keyword = value;
            }
        }

        public string GetReplacement(string scriptName)
        {
            if (replacementFunction == null)
            {
                return keyword;
            }
            return replacementFunction.GetReplacement(scriptName, scriptName);
        }
    }
}