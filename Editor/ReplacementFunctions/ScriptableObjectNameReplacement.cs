namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class ScriptableObjectNameReplacement : ReplacementFunction
    {
        public override string GetReplacement(string scriptName, string path)
        {
            return TemplateSettings.instance.GetScriptableObjectName();
        }
    }
}