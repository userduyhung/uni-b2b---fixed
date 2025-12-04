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
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

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
            // Convert UTC time to Vietnam time (UTC+7) before writing to JSON
            DateTime vietnamTime;
            
            if (value.Kind == DateTimeKind.Utc)
            {
                // If it's already UTC, convert to Vietnam time
                vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(value, VietnamTimeZone);
            }
            else if (value.Kind == DateTimeKind.Unspecified)
            {
                // If unspecified, assume it's UTC and convert
                var utcTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, VietnamTimeZone);
            }
            else
            {
                // If it's already local time, use as is
                vietnamTime = value;
            }

            // Write in ISO 8601 format with timezone offset
            writer.WriteStringValue(vietnamTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
        }
    }

    /// <summary>
    /// Custom JSON converter for nullable DateTime that automatically converts to Vietnam timezone
    /// </summary>
    public class VietnamNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

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

            // Convert UTC time to Vietnam time (UTC+7) before writing to JSON
            DateTime vietnamTime;
            var dateTime = value.Value;
            
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, VietnamTimeZone);
            }
            else if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                var utcTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, VietnamTimeZone);
            }
            else
            {
                vietnamTime = dateTime;
            }

            writer.WriteStringValue(vietnamTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
        }
    }
}
