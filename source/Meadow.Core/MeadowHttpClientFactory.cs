using System;
using System.Net.Http;

namespace Meadow;

/// <summary>
/// 
/// </summary>
public class MeadowHttpClientFactory : IMeadowHttpClientFactory
{
    /// <summary>
    /// Create a new HttpClient with a MeadowMessageHandler
    /// </summary>
    /// <returns></returns>
    public HttpClient CreateClient()
    {
        return new HttpClient(new MeadowHttpMessageHandler());
    }
}