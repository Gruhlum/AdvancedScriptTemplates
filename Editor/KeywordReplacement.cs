using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public class KeywordReplacement
    {
        public string keyword = "#KEYWORD#";
        [SubclassSelector, SerializeReference] public ReplacementFunction replacementFunction;
    }
}