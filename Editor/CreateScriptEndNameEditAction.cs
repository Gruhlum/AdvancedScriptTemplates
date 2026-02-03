using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HexTecGames.AdvancedScriptTemplates.Editor
{
    [System.Serializable]
    internal class CreateScriptEndNameEditAction : EndNameEditAction
    {
        public ScriptTemplateData templateData;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            WriteTemplate(pathName, templateData);

            if (templateData.otherItems != null)
            {
                string baseName = Path.GetFileNameWithoutExtension(pathName);

                foreach (var item in templateData.otherItems)
                {
                    string otherPath = pathName.Replace(baseName, baseName + item.suffix);
                    WriteTemplate(otherPath, item);
                    AssetDatabase.LoadAssetAtPath<Object>(otherPath);
                }
            }

            AssetDatabase.Refresh();

            Object obj = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            Selection.SetActiveObjectWithContext(obj, obj);
        }

        private void WriteTemplate(string path, ScriptTemplateData data)
        {
            string processed = ProcessTemplate(path, data);
            File.WriteAllText(path, processed);
        }

        private string ProcessTemplate(string path, ScriptTemplateData data)
        {
            string scriptName = Path.GetFileNameWithoutExtension(path);
            string text = data.template.text;

            text = ReplaceNamespacePlaceholder(path, text);

            foreach (KeywordReplacement replacement in data.keywordReplacements)
            {
                text = text.Replace(replacement.Keyword, replacement.GetReplacement(scriptName));
            }

            return text;
        }

        private string ReplaceNamespacePlaceholder(string path, string text)
        {
            TemplateSettings settings = TemplateSettings.instance;

            // If namespaces are disabled, remove placeholder entirely
            if (!settings.addNamespace)
            {
                return text.Replace("#NAMESPACE#", string.Empty);
            }

            string ns = settings.GenerateNamespaceName(path);

            // Split into lines
            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Find the placeholder line
            int placeholderIndex = Array.FindIndex(lines, l => l.Contains("#NAMESPACE#"));

            if (placeholderIndex < 0)
            {
                // No placeholder found, return original
                return text;
            }

            List<string> output = new List<string>();

            // 1. Add everything BEFORE the placeholder (this includes usings)
            for (int i = 0; i < placeholderIndex; i++)
            {
                output.Add(lines[i]);
            }

            // 2. Add namespace header + opening brace
            output.Add($"namespace {ns}");
            output.Add("{");

            // 3. Add everything AFTER the placeholder, indented
            for (int i = placeholderIndex + 1; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Length > 0)
                {
                    output.Add("    " + line);
                }
                else
                {
                    output.Add(string.Empty);
                }
            }

            // 4. Add closing brace
            output.Add("}");

            return string.Join(Environment.NewLine, output);
        }
    }
}