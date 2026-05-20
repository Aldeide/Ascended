using System;

namespace Core.Utilities
{
    public static class StringUtilities
    {
        /// <summary>
        /// Sanitizes a string by removing characters that could be interpreted as TextMeshPro rich text tags.
        /// This prevents UI layout breakage and text spoofing (XSS-equivalent in Unity).
        /// </summary>
        public static string SanitizeRichText(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Replace("<", "").Replace(">", "").Replace("\n", "").Replace("\r", "");
        }
    }
}
