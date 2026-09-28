using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Altirs.Sdk.Models;

namespace Altirs.Sdk
{
    public sealed class AltirsClient : IDisposable
    {
        // Taken from the assembly attribute the csproj <Version> generates, so
        // the version is declared once in Altirs.Sdk.csproj.
        private static readonly string SdkVersion =
            (typeof(AltirsClient).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "0.0.0")
            .Split('+')[0];

        private const string DefaultBaseUrl = "https://altirs.ai/api";
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly bool _ownsHttpClient;

        public AltirsClient(string apiKey, string? baseUrl = null, TimeSpan? timeout = null, HttpClient? httpClient = null)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _baseUrl = (baseUrl ?? DefaultBaseUrl).TrimEnd('/');

            if (httpClient != null)
            {
                _http = httpClient;
                _ownsHttpClient = false;
            }
            else
            {
                _http = new HttpClient { Timeout = timeout ?? DefaultTimeout };
                _ownsHttpClient = true;
            }
        }

        public async Task<CheckResponse> CheckAsync(CheckPayload payload, CancellationToken ct = default)
        {
            return await RequestAsync<CheckResponse>("/v1/guardrail/check", payload, ct);
        }

        public async Task<BatchResponse> BatchAsync(List<CheckPayload> items, CancellationToken ct = default)
        {
            var body = new BatchPayload { Items = items };
            return await RequestAsync<BatchResponse>("/v1/guardrail/batch", body, ct);
        }

        public async Task<bool> IsSafeAsync(string input, CancellationToken ct = default)
        {
            var result = await CheckAsync(new CheckPayload { Input = input }, ct);
            return result.Safe;
        }

        public async Task<string> MaskPiiAsync(string text, CancellationToken ct = default)
        {
            var result = await CheckAsync(new CheckPayload
            {
                Input = text,
                Checks = new List<string> { "pii" },
                Options = new CheckOptions { Pii = new PiiOptions { Mask = true } }
            }, ct);
            return result.MaskedInput ?? text;
        }

        private async Task<T> RequestAsync<T>(string path, object body, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + path);
            request.Content = content;
            request.Headers.Add("X-Api-Key", _apiKey);
            request.Headers.Add("X-SDK-Version", "csharp/" + SdkVersion);

            HttpResponseMessage response;
            try
            {
                response = await _http.SendAsync(request, ct);
            }
            catch (TaskCanceledException)
            {
                throw new AltirsException("Request timed out", 408, "timeout");
            }
            catch (Exception ex)
            {
                throw new AltirsException(ex.Message, 0, "unknown");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var err = JsonSerializer.Deserialize<ErrorResponse>(responseBody);
                throw new AltirsException(
                    err?.Error ?? $"Request failed with status {(int)response.StatusCode}",
                    (int)response.StatusCode,
                    err?.RequestId ?? "unknown"
                );
            }

            return JsonSerializer.Deserialize<T>(responseBody)!;
        }

        public void Dispose()
        {
            if (_ownsHttpClient) _http.Dispose();
        }
    }
}
