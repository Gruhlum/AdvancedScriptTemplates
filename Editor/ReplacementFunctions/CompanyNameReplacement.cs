using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class CompanyNameReplacement : ReplacementFunction
    {
        public override string GetReplacement(string scriptName, string path)
        {
            return Application.companyName;
        }
    }
}