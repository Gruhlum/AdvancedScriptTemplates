using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [CreateAssetMenu(menuName = "HexTecGames/AdvancedScriptTemplates/TemplateGroup")]
    public class TemplateGroup : ScriptableObject
    {
        public ScriptTemplateData mainData;
        public List<TemplateGroupItem> items = new List<TemplateGroupItem>();
    }
}