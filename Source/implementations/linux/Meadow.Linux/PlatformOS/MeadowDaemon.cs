using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Meadow;

internal static class MeadowDaemon
{
    private const string DaemonServiceAddress = "http://127.0.0.1";
    private const int DaemonServicePort = 5000;

    public static object? GetConfiguration()
    {
        // TODO:
        return null;
    }

    public static async Task ApplyUpdate()
    {
        // at this point the update is downloaded and expanded.  
        // we simply need to thell the daemon we're ready for it to apply the update and restart us
        try
        {
            // this is a rare use, so no need to keep it around
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri($"{DaemonServiceAddress}:{DaemonServicePort}/api/")
            };

            // TODO: should we send it the app assembly name here too?
            var payload = new JsonContent(new ApplyAction
            {
                Pid = Process.GetCurrentProcess().Id, // this is the PID of dotnet
                AppDir = AppDomain.CurrentDomain.BaseDirectory,
                Command = "dotnet", // this is a .dotnet app
                Executable = $"{Assembly.GetEntryAssembly()?.GetName().Name}.dll"
            });


            Resolver.Log.Info("Notifying Meadow Daemon to apply update and restart application...", "meadow-daemon");
            Resolver.Log.Debug($"ApplyUpdate Payload: {await payload.ReadAsStringAsync()}", "meadow-daemon");

            var response = await httpClient.PutAsync(Endpoints.Apply, payload);

            if (!response.IsSuccessStatusCode)
            {
                // TODO: raise/call error handler?

                var resp = await response.Content.ReadAsStringAsync();
                Resolver.Log.Error($"ApplyUpdate Error ({response.StatusCode}: {resp})", "meadow-daemon");
                // TODO: throw an appropriate exception
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"ApplyUpdate Exception: {ex.Message}", "meadow-daemon");
        }
    }

    private static class Endpoints
    {
        public static string DeviceInfo => "info";
        public static string Updates => "updates";
        public static string UpdateAction => "updates/{id}";
        public static string Apply => "apply";
    }

    private static class UpdateActions
    {
        public static string Apply => "apply";
        public static string Download => "download";
    }

    internal record ApplyAction
    {
        [JsonPropertyName("pid")]
        public int Pid { get; set; }
        [JsonPropertyName("command")]
        public string? Command { get; set; }
        [JsonPropertyName("app_dir")]
        public string? AppDir { get; set; }
        [JsonPropertyName("executable")]
        public string? Executable { get; set; }
    }

    internal record UpdateAction
    {
        [JsonPropertyName("action")]
        public string Action { get; set; } = default!;
        [JsonPropertyName("pid")]
        public int Pid { get; set; }
        [JsonPropertyName("app_dir")]
        public string? AppDirectory { get; set; }
        [JsonPropertyName("command")]
        public string? Command { get; set; }
    }

    internal class JsonContent : StringContent
    {
        public JsonContent(object content)
            : base(JsonSerializer.Serialize(content), Encoding.UTF8, "text/json")
        {
        }
    }
}
