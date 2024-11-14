﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using SafeStreet;
//
//    var crime = Crime.FromJson(jsonString);

namespace SafeStreet
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.Json.Serialization;
    using System.Text.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Crime
    {
        [JsonProperty("instanceid")]
        public string Instanceid { get; set; }

        [JsonProperty("incident_no")]
        [Newtonsoft.Json.JsonConverter(typeof(ParseStringConverter))]
        public object IncidentNo { get; set; }

        [JsonProperty("date_reported")]
        public DateTimeOffset DateReported { get; set; }

        [JsonProperty("date_from")]
        public DateTimeOffset DateFrom { get; set; }

        [JsonProperty("date_to")]
        public DateTimeOffset DateTo { get; set; }

        [JsonProperty("clsd")]
        public string Clsd { get; set; }

        [JsonProperty("ucr")]
        [Newtonsoft.Json.JsonConverter(typeof(ParseStringConverter))]
        public long Ucr { get; set; }

        [JsonProperty("dst")]
        [Newtonsoft.Json.JsonConverter(typeof(ParseStringConverter))]
        public object Dst { get; set; }

        [JsonProperty("beat")]
        [Newtonsoft.Json.JsonConverter(typeof(ParseStringConverter))]
        public object Beat { get; set; }

        [JsonProperty("offense")]
        public string Offense { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("dayofweek")]
        public string Dayofweek { get; set; }

        [JsonProperty("rpt_area")]
        [Newtonsoft.Json.JsonConverter(typeof(ParseStringConverter))]
        public object RptArea { get; set; }

        [JsonProperty("cpd_neighborhood")]
        public string CpdNeighborhood { get; set; }

        [JsonProperty("weapons")]
        public string Weapons { get; set; }

        [JsonProperty("date_of_clearance")]
        public DateTimeOffset DateOfClearance { get; set; }

        [JsonProperty("hour_from")]
        [Newtonsoft.Json.JsonConverter(typeof(ParseStringConverter))]
        public long HourFrom { get; set; }

        [JsonProperty("hour_to")]
        [Newtonsoft.Json.JsonConverter(typeof(ParseStringConverter))]
        public long HourTo { get; set; }

        [JsonProperty("address_x")]
        public string AddressX { get; set; }

        [JsonProperty("longitude_x")]
        public string LongitudeX { get; set; }

        [JsonProperty("latitude_x")]
        public string LatitudeX { get; set; }

        [JsonProperty("victim_age")]
        public string VictimAge { get; set; }

        [JsonProperty("victim_gender")]
        public string VictimGender { get; set; }

        [JsonProperty("suspect_age")]
        public string SuspectAge { get; set; }

        [JsonProperty("ucr_group")]
        public string UcrGroup { get; set; }

        [JsonProperty("zip")]
        [Newtonsoft.Json.JsonConverter(typeof(ParseStringConverter))]
        public long Zip { get; set; }

        [JsonProperty("community_council_neighborhood")]
        public string CommunityCouncilNeighborhood { get; set; }

        [JsonProperty("sna_neighborhood")]
        public string SnaNeighborhood { get; set; }

        // Helper method to get IncidentNo as a string
        public string GetIncidentNoAsString()
        {
            return IncidentNo?.ToString();
        }

        // Helper method to get Dst as a string
        public string GetDstAsString()
        {
            return Dst?.ToString();
        }

        // Helper method to get Beat as a string
        public string GetBeatAsString()
        {
            return Beat?.ToString();
        }
        public string GetRptAreaAsString()
        {
            return RptArea?.ToString();
        }
    }

    public partial class Crime
    {
        public static List<Crime> FromJson(string json) => JsonConvert.DeserializeObject<List<Crime>>(json, SafeStreet.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<Crime> self) => JsonConvert.SerializeObject(self, SafeStreet.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?) || t == typeof(object);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Integer)
            {
                // If it's an integer, return it as a long
                return (long)reader.Value;
            }

            if (reader.TokenType == JsonToken.String)
            {
                // If it's a string, attempt to parse it as a long
                var value = (string)reader.Value;
                if (long.TryParse(value, out long l))
                {
                    return l;
                }
                // If parsing fails, return the original string as a fallback
                return value;
            }

            // Fallback in case the token type is unexpected
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            if (untypedValue is long || untypedValue is int)
            {
                writer.WriteValue((long)untypedValue);
            }
            else
            {
                writer.WriteValue(untypedValue.ToString());
            }
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
