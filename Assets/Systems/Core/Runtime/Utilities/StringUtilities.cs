using Unity.Collections;

namespace Systems.Core.Utilities
{
    public static class StringUtilities
    {
        /// <summary>
        /// Sanitizes a FixedString64Bytes to prevent TextMeshPro rich text injection (XSS equivalent).
        /// Strips '<' and '>' characters.
        /// </summary>
        public static FixedString64Bytes SanitizeForRichText(FixedString64Bytes input)
        {
            string str = input.ToString();
            if (str.Contains("<") || str.Contains(">"))
            {
                str = str.Replace("<", "").Replace(">", "");
                return new FixedString64Bytes(str);
            }
            return input;
        }
    }
}
