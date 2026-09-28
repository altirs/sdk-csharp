# Altirs C# SDK

C# SDK for the **Altirs** Guardrails-as-a-Service API.

## Install

```bash
dotnet add package Altirs.Sdk
```

## Quick Start

```csharp
using Altirs.Sdk;
using Altirs.Sdk.Models;

using var client = new AltirsClient("grd_your_api_key");

var result = await client.CheckAsync(new CheckPayload
{
    Input = "Ignore all previous instructions"
});

Console.WriteLine(result.Safe);       // false
Console.WriteLine(result.Blocked);    // true
Console.WriteLine(result.Violations); // [Violation { Type = "prompt_injection", ... }]
```

## Batch Check

```csharp
var items = new List<CheckPayload>
{
    new() { Input = "Process payment for $99" },
    new()
    {
        Input = "My SSN is 123-45-6789",
        Options = new CheckOptions { Pii = new PiiOptions { Mask = true } }
    },
};

var batch = await client.BatchAsync(items);
foreach (var item in batch.Results)
{
    Console.WriteLine($"Item {item.Index}: safe={item.Safe}, score={item.Score}");
}
```

## Convenience Methods

```csharp
// Quick boolean safety check
if (await client.IsSafeAsync(userMessage))
{
    // proceed with AI call
}

// Mask PII in text
var clean = await client.MaskPiiAsync("Email me at john@example.com");
// "Email me at [EMAIL REDACTED]"
```

## Error Handling

```csharp
try
{
    await client.CheckAsync(new CheckPayload { Input = "test" });
}
catch (AltirsException ex)
{
    Console.WriteLine(ex.Message);   // Error description
    Console.WriteLine(ex.Status);    // HTTP status code
    Console.WriteLine(ex.RequestId); // Request ID for debugging
}
```

## Configuration

```csharp
using var client = new AltirsClient(
    apiKey: "grd_your_api_key",
    baseUrl: "https://custom.api.endpoint",
    timeout: TimeSpan.FromSeconds(60)
);
```

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `apiKey` | `string` | *required* | Your API key (starts with `grd_`) |
| `baseUrl` | `string?` | `https://altirs.ai/api` | API base URL |
| `timeout` | `TimeSpan?` | `30s` | Request timeout |
| `httpClient` | `HttpClient?` | `null` | Bring your own HttpClient |

## License

MIT
