using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [CreateAssetMenu(menuName = "HexTecGames/AdvancedScriptTemplates/KeywordReplacementCollection")]
    public class KeywordReplacementCollection : ScriptableObject
    {
        public List<KeywordReplacement> keywordReplacements;
    }
}