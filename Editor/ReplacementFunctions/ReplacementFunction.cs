namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public abstract class ReplacementFunction
    {
        public abstract string GetReplacement(string scriptName, string path);
    }
}