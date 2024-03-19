using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

/// <summary>
/// Meadow HttpMessage Handler
/// </summary>
public class MeadowHttpMessageHandler : DelegatingHandler
{
    /// <summary>
    /// 
    /// </summary>
    public MeadowHttpMessageHandler() : base(new HttpClientHandler()) { }

    /// <summary>
    /// Handler Meadow specific events
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Network subsystem is down"))
            {
                Resolver.Log.Error("Network subsystem error - restarting device");
                Resolver.Device.PlatformOS.Reset();
            }

            throw ex;
        }
    }
}