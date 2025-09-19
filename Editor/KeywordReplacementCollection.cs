using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class KeywordReplacementCollection : IEnumerable<KeywordReplacement>
    {
        public List<KeywordReplacement> keywordReplacements = new List<KeywordReplacement>() { new KeywordReplacement("#SCRIPT_NAME#") };

        public IEnumerator<KeywordReplacement> GetEnumerator()
        {
            return keywordReplacements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}