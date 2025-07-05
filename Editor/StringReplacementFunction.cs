using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class StringReplacementFunction : ReplacementFunction
    {
        [SerializeField] private string returnValue = default;

        public override string GetReplacement()
        {
            return returnValue;
        }
    }
}