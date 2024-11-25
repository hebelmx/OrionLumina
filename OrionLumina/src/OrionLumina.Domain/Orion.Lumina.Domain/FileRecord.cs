using System.Security.Cryptography;
using System.Text;

namespace Orion.Lumina.Domain;


public class FileRecord
{
    public Guid Id { get; set; }


    public string FileName { get; set; } = String.Empty;

    public string Content { get; set; } = String.Empty;

    public string Hash { get; set; } = String.Empty;

    public FileStatus Status { get; set; } = FileStatus.Default;


    public DateTime UploadedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }
    /// <summary>
    /// Generates a SHA-256 hash for the given input string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A hexadecimal string representing the hash.</returns>
    public static string ComputeSha256Hash(string input)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = sha256.ComputeHash(inputBytes);

        return ConvertToHexString(hashBytes);
    }

    /// <summary>
    /// Converts a byte array to a hexadecimal string.
    /// </summary>
    /// <param name="hashBytes">The byte array to convert.</param>
    /// <returns>A hexadecimal string.</returns>
    private static string ConvertToHexString(byte[] hashBytes)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2")); // Converts each byte to a lowercase hexadecimal string.
        }
        return sb.ToString();
    }

}