namespace Meadow
{
    public interface ILifecycleSettings
    {
        bool RestartOnAppFailure => true;
        int AppFailureRestartDelaySeconds => 5;
    }
}
