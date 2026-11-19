using System.Security.Cryptography;

namespace GymRegistrationApp.Services;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private const string Prefix = "PBKDF2-SHA256";

    public static string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return string.Join('$', Prefix, Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    public static bool Verify(string password, string? storedHash, string? legacyPassword = null)
    {
        if (!string.IsNullOrEmpty(storedHash) && TryParse(storedHash, out var iterations, out var salt, out var expectedKey))
        {
            var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedKey.Length);
            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }

        return legacyPassword is not null &&
               CryptographicOperations.FixedTimeEquals(
                   System.Text.Encoding.UTF8.GetBytes(password),
                   System.Text.Encoding.UTF8.GetBytes(legacyPassword));
    }

    public static bool IsValidPassword(string password, out string error)
    {
        if (password.Length < 8)
        {
            error = "Password must be at least 8 characters.";
            return false;
        }

        if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
        {
            error = "Password must include an uppercase letter, a lowercase letter, and a number.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static bool TryParse(string value, out int iterations, out byte[] salt, out byte[] key)
    {
        iterations = 0;
        salt = Array.Empty<byte>();
        key = Array.Empty<byte>();

        var parts = value.Split('$');
        if (parts.Length != 4 || !string.Equals(parts[0], Prefix, StringComparison.Ordinal) ||
            !int.TryParse(parts[1], out iterations) || iterations < 1)
        {
            return false;
        }

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            key = Convert.FromBase64String(parts[3]);
            return salt.Length >= SaltSize && key.Length >= 16;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
