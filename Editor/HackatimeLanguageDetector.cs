using System;
using System.IO;

namespace Blizz606.UnityHackatime
{
    internal static class HackatimeLanguageDetector
    {
        internal static string FromPath(string path)
        {
            string extension = Path.GetExtension(path ?? string.Empty).ToLowerInvariant();
            switch (extension)
            {
                case ".cs": return "C#";
                case ".shader": case ".cginc": return "ShaderLab";
                case ".hlsl": return "HLSL";
                case ".json": return "JSON";
                case ".md": return "Markdown";
                case ".xml": return "XML";
                case ".yaml": case ".yml": return "YAML";
                case ".uxml": return "UXML";
                case ".uss": return "USS";
                case ".unity": case ".prefab": case ".asset": case ".mat": case ".controller": case ".anim": return "Unity";
                default: return string.IsNullOrEmpty(extension) ? "Unity" : extension.TrimStart('.');
            }
        }
    }
}
