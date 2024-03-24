using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

/// <summary>
/// Extensions for Tasks
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Force a faulted Task to re-throw its exception on the caller thread.
    /// </summary>
    /// <param name="task">The target Task</param>
    /// <remarks>Use this for unmonitored Tasks to bubble exceptions up to the AppDomain UnhandledExceptionHandler</remarks>
    public static void RethrowUnhandledExceptions(this Task task)
    {
        task.ContinueWith(t =>
        {
            var message = $"Task ID {t?.Id ?? 0} faulted";

            if (t?.Exception != null)
            {
                message += $": {t.Exception.Message}";
                //throw t.Exception;
            }

            Resolver.Log.Error(message);
            MeadowOS.TerminateRun();

        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    /// <summary>
    /// Force a faulted Task to re-throw its exception on the caller thread.
    /// </summary>
    /// <param name="task">The target Task</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <remarks>Use this for unmonitored Tasks to bubble exceptions up to the AppDomain UnhandledExceptionHandler</remarks>
    public static void RethrowUnhandledExceptions(this Task task, CancellationToken cancellationToken)
    {
        task.ContinueWith((t, s) =>
        {
            throw t.Exception;
        }, TaskContinuationOptions.OnlyOnFaulted, cancellationToken);
    }
}
