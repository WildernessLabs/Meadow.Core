namespace Profiler
{
    class Program
    {
        static ProfilerApp _app;

        static void Main(string[] args)
        {
            _app = new ProfilerApp();
            _app.Run();
        }
    }
}
