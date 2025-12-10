using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace B2BMarketplace.Api.Configuration
{
    /// <summary>
    /// Custom JSON converter that automatically converts UTC DateTime to Vietnam timezone (UTC+7)
    /// This ensures all DateTime fields returned by API are in local Vietnam time
    /// </summary>
    public class VietnamDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneHelper.VietnamTimeZone;

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // When reading from JSON, parse as UTC
            var dateTimeString = reader.GetString();
            if (DateTime.TryParse(dateTimeString, out var dateTime))
            {
                // If the DateTime has no timezone info, assume it's UTC
                if (dateTime.Kind == DateTimeKind.Unspecified)
                {
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                }
                return dateTime;
            }
            throw new JsonException($"Unable to parse DateTime: {dateTimeString}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Normalize input to a DateTimeOffset representing the instant in UTC
            DateTimeOffset utcDto;

            if (value.Kind == DateTimeKind.Utc)
            {
                utcDto = new DateTimeOffset(value);
            }
            else if (value.Kind == DateTimeKind.Local)
            {
                utcDto = new DateTimeOffset(value).ToUniversalTime();
            }
            else // Unspecified - assume it's UTC (existing behavior)
            {
                var assumedUtc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                utcDto = new DateTimeOffset(assumedUtc);
            }

            // Convert UTC instant to Vietnam time zone offset-aware DateTimeOffset
            var vietOffset = VietnamTimeZone.GetUtcOffset(utcDto.UtcDateTime);
            var vietDto = utcDto.ToOffset(vietOffset);

            // Write in ISO 8601 round-trip format with offset
            writer.WriteStringValue(vietDto.ToString("o"));
        }
    }

    /// <summary>
    /// Custom JSON converter for nullable DateTime that automatically converts to Vietnam timezone
    /// </summary>
    public class VietnamNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneHelper.VietnamTimeZone;

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateTimeString = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                return null;
            }

            if (DateTime.TryParse(dateTimeString, out var dateTime))
            {
                if (dateTime.Kind == DateTimeKind.Unspecified)
                {
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                }
                return dateTime;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            var dateTime = value.Value;

            DateTimeOffset utcDto;
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                utcDto = new DateTimeOffset(dateTime);
            }
            else if (dateTime.Kind == DateTimeKind.Local)
            {
                utcDto = new DateTimeOffset(dateTime).ToUniversalTime();
            }
            else
            {
                var assumedUtc = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                utcDto = new DateTimeOffset(assumedUtc);
            }

            var vietOffset = VietnamTimeZone.GetUtcOffset(utcDto.UtcDateTime);
            var vietDto = utcDto.ToOffset(vietOffset);
            writer.WriteStringValue(vietDto.ToString("o"));
        }
    }
}

    internal static class TimeZoneHelper
    {
        internal static readonly TimeZoneInfo VietnamTimeZone = CreateVietnamTimeZone();

        private static TimeZoneInfo CreateVietnamTimeZone()
        {
            // Try Windows ID first, then IANA. Fallback to UTC.
            var windowsId = "SE Asia Standard Time";
            var ianaId = "Asia/Ho_Chi_Minh";
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
            }
            catch
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(ianaId);
                }
                catch
                {
                    return TimeZoneInfo.Utc;
                }
            }
        }
    }
