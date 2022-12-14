namespace Meadow
{
    public interface ILifecycleSettings
    {
        bool RestartOnAppFailure => false;
        int AppFailureRestartDelaySeconds => 5;
    }
}
