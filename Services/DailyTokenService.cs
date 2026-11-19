

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace GymRegistrationApp.Services;

/// <summary>
/// Generates and validates daily QR attendance tokens.
/// Uses Manila time and supports optional gym coordinates.
/// </summary>
public static class DailyTokenService
{
    private const string SecretSalt =
        "UniversalGym_Attendance_AntiFraud_Secret_2026";

    /// <summary>
    /// Generates a 16-character daily attendance token.
    /// </summary>
    public static string GenerateDailyToken(
        DateTime date,
        double gymLat = 0,
        double gymLng = 0)
    {
        var latStr = Math.Abs(gymLat) > 0.0001
            ? gymLat.ToString("F4", CultureInfo.InvariantCulture)
            : "";

        var lngStr = Math.Abs(gymLng) > 0.0001
            ? gymLng.ToString("F4", CultureInfo.InvariantCulture)
            : "";

        var raw =
            $"{SecretSalt}_{date:yyyy-MM-dd}_{latStr}_{lngStr}";

        using var sha = SHA256.Create();

        var bytes = sha.ComputeHash(
            Encoding.UTF8.GetBytes(raw)
        );

        return Convert.ToHexString(bytes).Substring(0, 16);
    }

    /// <summary>
    /// Validates the QR token against today's date
    /// using Philippine time.
    /// </summary>
    public static bool ValidateDailyToken(
        string? dateString,
        string? token,
        double gymLat,
        double gymLng,
        out string errorMessage)
    {
        errorMessage = "";

        if (string.IsNullOrWhiteSpace(dateString) ||
            string.IsNullOrWhiteSpace(token))
        {
            errorMessage =
                "Invalid QR code: Missing daily security token. " +
                "Please scan the current QR code at the gym.";

            return false;
        }

        if (!DateTime.TryParseExact(
            dateString,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate))
        {
            errorMessage =
                "Invalid QR code: Malformed date in QR link.";

            return false;
        }

        var todayManila = AttendanceService.ManilaNow().Date;

        if (parsedDate.Date < todayManila)
        {
            errorMessage =
                "Expired QR Code: Please scan today's active QR code.";

            return false;
        }

        if (parsedDate.Date > todayManila)
        {
            errorMessage =
                "Invalid QR Code: The QR date is from the future.";

            return false;
        }

        var expectedToken = GenerateDailyToken(
            todayManila,
            gymLat,
            gymLng
        );

        if (!string.Equals(
            token.Trim(),
            expectedToken,
            StringComparison.OrdinalIgnoreCase))
        {
            errorMessage =
                "Invalid QR Code: Security verification failed.";

            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates QR tokens without GPS coordinates.
    /// </summary>
    public static bool ValidateDailyToken(
        string? dateString,
        string? token,
        out string errorMessage)
    {
        return ValidateDailyToken(
            dateString,
            token,
            0,
            0,
            out errorMessage
        );
    }

    /// <summary>
    /// Calculates distance between GPS coordinates in meters.
    /// </summary>
    public static double CalculateDistanceMeters(
        double lat1,
        double lon1,
        double lat2,
        double lon2)
    {
        const double R = 6371000;

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) *
            Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(
            Math.Sqrt(a),
            Math.Sqrt(1 - a)
        );

        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}


