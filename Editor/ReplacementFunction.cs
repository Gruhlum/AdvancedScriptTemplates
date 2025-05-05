using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    public abstract class ReplacementFunction
    {
        public abstract string GetReplacement();
    }
}