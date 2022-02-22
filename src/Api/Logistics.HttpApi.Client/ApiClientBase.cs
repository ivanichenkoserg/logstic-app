﻿using Logistics.WebApi.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace Logistics.WebApi.Client;

internal abstract class ApiClientBase
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions serializerOptions;

    protected ApiClientBase(string host)
    {
        if (string.IsNullOrEmpty(host))
            throw new ArgumentNullException(host);

        try
        {
            var handler = new HttpClientHandler
            {
                // Disable SSL validation
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            httpClient = new HttpClient(handler) { BaseAddress = new Uri(host) };
        }
        catch (PlatformNotSupportedException)
        {
            httpClient = new HttpClient { BaseAddress = new Uri(host) };
        }
    }

    protected async Task<TResponse> GetRequestAsync<TResponse>(string endpoint,
        IDictionary<string, string> queries = default!)
        where TResponse : new()
    {
        var content = await GetRequestAsync(endpoint, queries);
        var deserializedObj = Deserialize<TResponse>(content) ?? default(TResponse);

        if (deserializedObj is null)
        {
            throw new ApiException(
                $"Serialization error, failed to deserialize object from response content:\n{content}");
        }

        return deserializedObj;
    }

    protected async Task<string> GetRequestAsync(string endpoint,
        IDictionary<string, string> queries = default!)
    {
        var requestEndpoint = endpoint;
        if (queries.Count > 0)
            requestEndpoint = QueryUtils.BuildQueryParameters(endpoint, queries);

        try
        {
            using var response = await httpClient.GetAsync(requestEndpoint);
            var content = await ReadContentAsync(response);
            return content;
        }
        catch (HttpRequestException e)
        {
            throw new ApiException(e.Message);
        }
    }

    protected async Task<TResponse> PostRequestAsync<TResponse, TRequest>(string endpoint, TRequest body)
        where TResponse : new()
        where TRequest : new()
    {
        var content = await PostRequestAsync(endpoint, body);
        var deserializedObj = Deserialize<TResponse>(content) ?? default(TResponse);

        if (deserializedObj is null)
        {
            throw new ApiException(
                $"Serialization error, failed to deserialize object from response content:\n{content}");
        }

        return deserializedObj;
    }

    protected async Task<string> PostRequestAsync<TRequest>(string endpoint, TRequest body)
    {
        try
        {
            using var response = await httpClient.PostAsync(endpoint, GetJsonContent(body));
            var content = await ReadContentAsync(response);
            return content;
        }
        catch (HttpRequestException e)
        {
            throw new ApiException(e.Message);
        }
    }

    private async Task<string> ReadContentAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var errorResult = Deserialize<Error>(content) ?? new Error(content, (int)response.StatusCode);
            throw new ApiException(errorResult.Message, (int)response.StatusCode);
        }

        return content;
    }

    private static HttpContent GetJsonContent<T>(T data)
    {
        var jsonData = JsonSerializer.Serialize(data);
        return new StringContent(jsonData, Encoding.UTF8, "application/json");
    }

    private static T? Deserialize<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}
