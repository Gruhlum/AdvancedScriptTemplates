namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class ProjectNameReplacement : ReplacementFunction
    {
        public override string GetReplacement(string scriptName, string path)
        {
            return TemplateSettings.instance.GetProjectName(path);
        }
    }
}