using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Altirs.Sdk.Models
{
    public class Violation
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("category")]
        public string Category { get; set; } = "";

        [JsonPropertyName("severity")]
        public string Severity { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
    }

    public class CheckOptions
    {
        [JsonPropertyName("pii")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PiiOptions? Pii { get; set; }

        [JsonPropertyName("hallucination")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HallucinationOptions? Hallucination { get; set; }
    }

    public class PiiOptions
    {
        [JsonPropertyName("mask")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Mask { get; set; }
    }

    public class HallucinationOptions
    {
        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Context { get; set; }
    }

    public class CheckPayload
    {
        [JsonPropertyName("input")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Input { get; set; }

        [JsonPropertyName("response")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Response { get; set; }

        [JsonPropertyName("checks")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Checks { get; set; }

        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CheckOptions? Options { get; set; }
    }

    public class CheckResponse
    {
        [JsonPropertyName("safe")]
        public bool Safe { get; set; }

        [JsonPropertyName("blocked")]
        public bool Blocked { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("violations")]
        public List<Violation> Violations { get; set; } = new();

        [JsonPropertyName("maskedInput")]
        public string? MaskedInput { get; set; }

        [JsonPropertyName("maskedResponse")]
        public string? MaskedResponse { get; set; }

        [JsonPropertyName("recommendation")]
        public string? Recommendation { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; } = "";

        [JsonPropertyName("latencyMs")]
        public int LatencyMs { get; set; }
    }

    public class BatchPayload
    {
        [JsonPropertyName("items")]
        public List<CheckPayload> Items { get; set; } = new();
    }

    public class BatchResultItem
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("safe")]
        public bool Safe { get; set; }

        [JsonPropertyName("blocked")]
        public bool Blocked { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("violations")]
        public List<Violation> Violations { get; set; } = new();

        [JsonPropertyName("maskedInput")]
        public string? MaskedInput { get; set; }

        [JsonPropertyName("maskedResponse")]
        public string? MaskedResponse { get; set; }

        [JsonPropertyName("recommendation")]
        public string? Recommendation { get; set; }

        [JsonPropertyName("latencyMs")]
        public int LatencyMs { get; set; }
    }

    public class BatchResponse
    {
        [JsonPropertyName("results")]
        public List<BatchResultItem> Results { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; } = "";

        [JsonPropertyName("latencyMs")]
        public int LatencyMs { get; set; }
    }

    internal class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = "";

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; } = "";
    }
}
