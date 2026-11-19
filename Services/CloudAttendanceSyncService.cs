


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GymRegistrationApp.Models;

namespace GymRegistrationApp.Services;

/// <summary>
/// Synchronizes cloud attendance records from Vercel/Supabase
/// into the local gym database.
/// </summary>
public static class CloudAttendanceSyncService
{
    private static readonly HttpClient _http = new()
    {
        Timeout = TimeSpan.FromSeconds(20)
    };

    public static async Task<SyncResult> SyncFromCloudAsync(string cloudBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(cloudBaseUrl))
        {
            return new SyncResult(
                false,
                "No cloud attendance URL configured.",
                0
            );
        }

        // This must match SYNC_API_KEY in Vercel.
        // Store it in a Windows environment variable,
        // not directly in the source code.
        var syncApiKey = Environment.GetEnvironmentVariable(
            "GYM_CLOUD_SYNC_API_KEY",
            EnvironmentVariableTarget.User
        );

        if (string.IsNullOrWhiteSpace(syncApiKey))
        {
            return new SyncResult(
                false,
                "Cloud sync API key is not configured on this computer.",
                0
            );
        }

        try
        {
            var cleanBase = cloudBaseUrl.Split('?')[0].TrimEnd('/');

            if (cleanBase.EndsWith(
                "/attendance",
                StringComparison.OrdinalIgnoreCase))
            {
                cleanBase = cleanBase.Substring(
                    0,
                    cleanBase.Length - "/attendance".Length
                );
            }

            var syncEndpoint = $"{cleanBase}/api/sync";
            var ackEndpoint = $"{cleanBase}/api/sync/ack";

            // Request attendance records using authorization.
            using var syncRequest = new HttpRequestMessage(
                HttpMethod.Get,
                syncEndpoint
            );

            syncRequest.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    syncApiKey
                );

            using var response = await _http.SendAsync(syncRequest);

            if (!response.IsSuccessStatusCode)
            {
                return new SyncResult(
                    false,
                    $"Cloud server returned status: {response.StatusCode}",
                    0
                );
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("records", out var recordsElement) ||
                recordsElement.ValueKind != JsonValueKind.Array)
            {
                return new SyncResult(
                    false,
                    "Cloud response did not contain a valid records list.",
                    0
                );
            }

            var syncedIds = new List<string>();
            var newRecordsCount = 0;

            foreach (var item in recordsElement.EnumerateArray())
            {
                var cloudId = GetString(item, "id");
                var memberId = GetString(item, "memberId");
                var fullName = GetString(item, "fullName");
                var firstName = GetString(item, "firstName");
                var lastName = GetString(item, "lastName");
                var dateStr = GetString(item, "attendanceDate");
                var recordedAtStr = GetString(item, "recordedAt");
                var checkInTimeStr = GetString(item, "checkInTime");

                // Do not acknowledge records with invalid IDs.
                if (string.IsNullOrWhiteSpace(cloudId) ||
                    string.IsNullOrWhiteSpace(memberId))
                {
                    continue;
                }

                // Validate the attendance date.
                if (!DateTime.TryParseExact(
                    dateStr,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var recordDate))
                {
                    continue;
                }

                // Use the original cloud check-in timestamp.
                var checkInTime = recordDate.Date;

                if (DateTimeOffset.TryParse(
                    recordedAtStr,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out var cloudTimestamp))
                {
                    try
                    {
                        var manilaZone = TimeZoneInfo.FindSystemTimeZoneById(
                            "Asia/Manila"
                        );

                        checkInTime = TimeZoneInfo.ConvertTime(
                            cloudTimestamp,
                            manilaZone
                        ).DateTime;
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        checkInTime = cloudTimestamp.ToOffset(
                            TimeSpan.FromHours(8)
                        ).DateTime;
                    }
                }
                else if (DateTime.TryParse(
                    checkInTimeStr,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedTime))
                {
                    checkInTime = recordDate.Date.Add(parsedTime.TimeOfDay);
                }

                // Check if attendance already exists locally.
                var alreadyExists = DataStore.AttendanceRecords.Any(a =>
                    !a.IsDeleted &&
                    a.MemberId.Equals(
                        memberId,
                        StringComparison.OrdinalIgnoreCase
                    ) &&
                    a.AttendanceDate.Date == recordDate.Date
                );

                if (!alreadyExists)
                {
                    // Look up registered member information.
                    var dbMember = DataStore.Members.FirstOrDefault(m =>
                        m.Id.Equals(
                            memberId,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                    if (dbMember != null)
                    {
                        fullName = dbMember.FullName;
                        firstName = dbMember.FirstName;
                        lastName = dbMember.LastName;
                    }
                    else if (string.IsNullOrWhiteSpace(firstName) &&
                             string.IsNullOrWhiteSpace(lastName))
                    {
                        // Supabase may only return the full name.
                        var nameParts = fullName.Split(
                            ' ',
                            2,
                            StringSplitOptions.RemoveEmptyEntries
                        );

                        firstName = nameParts.Length > 0
                            ? nameParts[0]
                            : "";

                        lastName = nameParts.Length > 1
                            ? nameParts[1]
                            : "";
                    }

                    var newAtt = new AttendanceRecord
                    {
                        Id = cloudId,
                        MemberId = memberId,
                        MemberName = fullName,
                        AttendanceDate = recordDate.Date,
                        CheckInTime = checkInTime,
                        RecordedBy = GetString(item, "recordedBy") is { Length: > 0 } recordedBy
                            ? recordedBy
                            : "QR-Mobile"
                    };

                    DataStore.AttendanceRecords.Add(newAtt);

                    // Persist locally before acknowledging this record.
                    if (!DataStore.TrySaveAttendanceRecords())
                    {
                        DataStore.AttendanceRecords.Remove(newAtt);
                        return new SyncResult(
                            false,
                            "Unable to save cloud attendance to the local database. Cloud records were not acknowledged.",
                            newRecordsCount
                        );
                    }

                    newRecordsCount++;
                }

                // Existing or successfully saved records
                // can now be acknowledged.
                syncedIds.Add(cloudId);
            }

            // Acknowledge records to Supabase through Vercel.
            if (syncedIds.Count > 0)
            {
                var ackPayload = JsonSerializer.Serialize(
                    new { syncedIds }
                );

                using var ackRequest = new HttpRequestMessage(
                    HttpMethod.Post,
                    ackEndpoint
                );

                ackRequest.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        syncApiKey
                    );

                ackRequest.Content = new StringContent(
                    ackPayload,
                    Encoding.UTF8,
                    "application/json"
                );

                using var ackResponse = await _http.SendAsync(ackRequest);

                if (!ackResponse.IsSuccessStatusCode)
                {
                    return new SyncResult(
                        false,
                        $"Attendance saved locally, but cloud acknowledgment failed: {ackResponse.StatusCode}",
                        newRecordsCount
                    );
                }
            }

            var message = newRecordsCount > 0
                ? $"Successfully synced {newRecordsCount} attendance record(s) from Cloud!"
                : "Cloud attendance is up to date (no new records).";

            return new SyncResult(
                true,
                message,
                newRecordsCount
            );
        }
        catch (Exception ex)
        {
            return new SyncResult(
                false,
                $"Sync error: {ex.Message}",
                0
            );
        }
    }

    private static string GetString(JsonElement item, string propertyName)
    {
        if (item.TryGetProperty(propertyName, out var property) &&
            property.ValueKind == JsonValueKind.String)
        {
            return property.GetString() ?? "";
        }

        return "";
    }

    public sealed record SyncResult(
        bool Success,
        string Message,
        int SyncedCount
    );
}


