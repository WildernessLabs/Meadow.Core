namespace Meadow
{
    public class LinuxApp<T> : App<T>
        where T : class, IMeadowDevice, new()
    {
    }
}
