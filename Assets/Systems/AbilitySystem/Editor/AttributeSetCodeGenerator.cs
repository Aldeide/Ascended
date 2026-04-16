using System.IO;
using System.Text;
using UnityEditor;

namespace AbilitySystem.Editor
{
    public static class AttributeSetCodeGenerator
    {
        private static string GetOutputPath()
        {
            // Ensure path is relative to Project Root for Unity API or Absolute for System.IO
            return Path.Combine(UnityEngine.Application.dataPath, "Systems/AbilitySystem/Generated/AttributeSets");
        }

        public static void Generate(AttributeSetData data)
        {
            string outputPath = GetOutputPath();
            UnityEngine.Debug.Log($"Generating AttributeSets at: {outputPath}");

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            foreach (var set in data.AttributeSets)
            {
                GenerateAttributeSetFile(set, outputPath);
            }

            AssetDatabase.Refresh();
        }

        private static void GenerateAttributeSetFile(AttributeSetDefinition set, string outputPath)
        {
            var sb = new StringBuilder();
            var className = $"{set.SetName}AttributeSet";
            var filePath = Path.Combine(outputPath, $"{className}.cs");

            UnityEngine.Debug.Log($"Writing file: {filePath}");

            sb.AppendLine("// -- AUTO-GENERATED --");
            sb.AppendLine("using AbilitySystem.Runtime.AttributeSets;");
            sb.AppendLine("using AbilitySystem.Runtime.Attributes;");
            sb.AppendLine("using AbilitySystem.Runtime.Core;");
            sb.AppendLine("");
            sb.AppendLine("namespace AbilitySystem.Generated.AttributeSets");
            sb.AppendLine("{");
            sb.AppendLine($"    public partial class {className} : AttributeSet");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {className}(IAbilitySystem owner) : base(owner)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine("");

            foreach (var attr in set.Attributes)
            {
                sb.AppendLine($"        public Attribute {attr} {{ get; set; }}");
            }

            sb.AppendLine("");
            sb.AppendLine("        public override void Reset()");
            sb.AppendLine("        {");
            foreach (var attr in set.Attributes)
            {
                sb.AppendLine($"            {attr}?.SetBaseValue(0f);");
                sb.AppendLine($"            {attr}?.SetCurrentValue(0f);");
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
