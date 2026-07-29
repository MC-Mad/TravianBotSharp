using System.Text.RegularExpressions;

namespace MainCore.Common.Extensions
{
    public static partial class StringExtension
    {
        [GeneratedRegex(@"[^a-zA-Z0-9\s]")]
        private static partial Regex NonAlphanumericRegex();

        [GeneratedRegex(@"[^a-zA-Z0-9._-]")]
        private static partial Regex UnsafeFolderNameRegex();

        public static string Sanitize(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return NonAlphanumericRegex().Replace(input, "").Replace(' ', '_');
        }

        public static string ToFolderName(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "_";

            var folderName = UnsafeFolderNameRegex()
                .Replace(input, "_")
                .Trim('.');

            return string.IsNullOrEmpty(folderName) ? "_" : folderName;
        }

        public static string GetServerUrl(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            if (!Uri.TryCreate(input, UriKind.Absolute, out var uri))
                return "";

            return $"{uri.Scheme}://{uri.Host}";
        }
    }
}