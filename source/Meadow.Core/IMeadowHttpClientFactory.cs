using System.Net.Http;

namespace Meadow;

/// <summary>
/// 
/// </summary>
public interface IMeadowHttpClientFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    HttpClient CreateClient();
}