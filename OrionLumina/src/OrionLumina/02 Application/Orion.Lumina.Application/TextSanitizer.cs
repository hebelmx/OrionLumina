using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;

namespace Orion.Lumina.Application;
[Flags]
public enum SanitizationOptions
{
    None = 0,
    RemoveEmails = 1,
    RemoveUrls = 2,
    RemoveHtmlTags = 4,
    RemoveExtraSpaces = 8,
    RemoveSpecialCharacters = 16,
    RemoveAllSpecialCharacters = 32


}


public static class TextSanitizer
{
    public static string SanitizeText(string input, SanitizationOptions options =
         SanitizationOptions.RemoveHtmlTags
        | SanitizationOptions.RemoveExtraSpaces
        )
    {
        string sanitized = input;
        // Sanitize based on the provided options
        if (options.HasFlag(SanitizationOptions.RemoveEmails))
        {
            sanitized = Regex.Replace(sanitized, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", "[EMAIL]");
        }

        if (options.HasFlag(SanitizationOptions.RemoveUrls))
        {
            sanitized = Regex.Replace(sanitized, @"https?://[^\s]+", "[URL]");
        }

        if (options.HasFlag(SanitizationOptions.RemoveHtmlTags))
        {
            sanitized = Regex.Replace(sanitized, "<.*?>", string.Empty);
        }

        if (options.HasFlag(SanitizationOptions.RemoveExtraSpaces))
        {
            sanitized = Regex.Replace(sanitized, @"\s+", " ");
        }

        if (options.HasFlag(SanitizationOptions.RemoveSpecialCharacters))
        {
            // Removes special characters except those allowed in emails and URLs
            sanitized = Regex.Replace(sanitized, @"[^a-zA-Z0-9\s@._%+-:/]", string.Empty);
        }

        if (options.HasFlag(SanitizationOptions.RemoveAllSpecialCharacters))
        {
            sanitized = Regex.Replace(sanitized, @"[^a-zA-Z0-9\s]", string.Empty);
        }

        return sanitized.Trim();
    }

   
}