using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [CreateAssetMenu(menuName = "HexTecGames/AdvancedScriptTemplates/KeywordReplacementCollection")]
    public class KeywordReplacementCollection : ScriptableObject, IEnumerable<KeywordReplacement>
    {
        public List<KeywordReplacement> keywordReplacements;


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