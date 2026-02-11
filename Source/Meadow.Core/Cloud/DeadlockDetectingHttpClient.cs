using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

internal class DeadlockDetectingHttpClient : HttpClient
{
    public event EventHandler? SendDeadlocked;

    public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sendTask = base.SendAsync(request, cancellationToken);
        var timeoutTask = Task.Delay(TimeSpan.FromMinutes(3));
        var completedTask = await Task.WhenAny(sendTask, timeoutTask);

        if (completedTask == timeoutTask)
        {
            SendDeadlocked?.Invoke(this, EventArgs.Empty);
            Resolver.Log.Error($"ReliableHttpClient.SendAsync is deadlocked!", "cloud");
            sendTask.Dispose();
            throw new TaskCanceledException();
        }

        return sendTask.Result;
    }
}
