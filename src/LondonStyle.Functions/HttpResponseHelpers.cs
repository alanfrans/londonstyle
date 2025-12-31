using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace LondonStyle.Functions;

internal static class HttpResponseHelpers
{
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static void AddPrivacyHeaders(HttpResponseData res)
    {
        res.Headers.Add("X-Content-Type-Options", "nosniff");
        res.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        res.Headers.Add("X-Data-Retention", "ephemeral");
        res.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, private");
        res.Headers.Add("Content-Type", "application/json; charset=utf-8");
    }

    public static async Task<HttpResponseData> Json(HttpRequestData req, HttpStatusCode statusCode, object body)
    {
        var res = req.CreateResponse(statusCode);
        AddPrivacyHeaders(res);
        await res.WriteStringAsync(JsonSerializer.Serialize(body, JsonOptions));
        return res;
    }
}
