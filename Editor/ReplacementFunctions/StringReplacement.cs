using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class StringReplacement : ReplacementFunction
    {
        [SerializeField] private string returnValue = default;

        public override string GetReplacement(string scriptName, string path)
        {
            return returnValue;
        }
    }
}