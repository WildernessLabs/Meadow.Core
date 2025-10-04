using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http;

internal static class HttpResponseHeadersExtensions
{
    public static IList<ContentDigestItem> GetContentDigests(this HttpResponseHeaders headers)
    {
        if (headers == null || !headers.TryGetValues("Content-Digest", out var digestHeaders))
        {
            return Array.Empty<ContentDigestItem>();
        }

        var items = new List<ContentDigestItem>();
        foreach (var header in digestHeaders)
        {
            var values = header.Split(',');
            foreach (var value in values)
            {
                var parts = value.Split('=');
                if (parts.Length > 1)
                {
                    items.Add(new ContentDigestItem(parts[0].Trim(), parts[1].Trim()));
                }
            }
        }

        return items;
    }
}

internal class ContentDigestItem(string algorithm, string value)
{
    public string Algorithm { get; set; } = algorithm;
    public string Value { get; set; } = value;
}
