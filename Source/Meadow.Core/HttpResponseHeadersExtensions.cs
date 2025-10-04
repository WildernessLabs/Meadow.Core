using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http;

internal static class HttpResponseHeadersExtensions
{
    public static IEnumerable<ContentDigestItem> GetContentDigests(this HttpResponseHeaders headers)
    {
        if (headers == null || !headers.TryGetValues("Content-Digest", out var digestHeaders))
        {
            yield break;
        }

        foreach (var header in digestHeaders)
        {
            var values = header.Split(',');
            foreach (var value in values)
            {
                var items = value.Split('=');
                if (items.Length > 1)
                {
                    yield return new ContentDigestItem(items[0].Trim(), items[1].Trim());
                }
            }
        }
    }
}

internal class ContentDigestItem(string algorithm, string value)
{
    public string Algorithm { get; set; } = algorithm;
    public string Value { get; set; } = value;
}
