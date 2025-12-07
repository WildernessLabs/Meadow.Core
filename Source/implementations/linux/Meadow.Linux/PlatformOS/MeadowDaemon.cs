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

    public static async Task<DaemonConfig?> GetConfiguration()
    {
        try
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri($"{DaemonServiceAddress}:{DaemonServicePort}/api/")
            };

            Resolver.Log.Debug($"Fetching daemon configuration from {DaemonServiceAddress}:{DaemonServicePort}/api/{Endpoints.DeviceInfo}", "meadow-daemon");

            var response = await httpClient.GetAsync(Endpoints.DeviceInfo).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Resolver.Log.Error($"Failed to fetch daemon configuration ({response.StatusCode}): {errorContent}", "meadow-daemon");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var info = JsonSerializer.Deserialize<DaemonInfoResponse>(content);

            if (info?.Config == null)
            {
                Resolver.Log.Warn("Daemon info response did not contain config", "meadow-daemon");
                return null;
            }

            Resolver.Log.Debug($"Successfully retrieved daemon configuration", "meadow-daemon");
            return info.Config;
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Exception while fetching daemon configuration: {ex.Message}", "meadow-daemon");
            return null;
        }
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
            Resolver.Log.Debug($"ApplyUpdate Payload: {await payload.ReadAsStringAsync().ConfigureAwait(false)}", "meadow-daemon");

            var response = await httpClient.PutAsync(Endpoints.Apply, payload).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // TODO: raise/call error handler?

                var resp = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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

    internal record DaemonConfig
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("enable_mqtt_listener")]
        public bool EnableMqttListener { get; set; }

        [JsonPropertyName("meadow_root")]
        public string? MeadowRoot { get; set; }

        [JsonPropertyName("meadow_temp")]
        public string? MeadowTemp { get; set; }

        [JsonPropertyName("update_store_path")]
        public string? UpdateStorePath { get; set; }

        [JsonPropertyName("temp_extract_path")]
        public string? TempExtractPath { get; set; }

        [JsonPropertyName("staging_path")]
        public string? StagingPath { get; set; }

        [JsonPropertyName("rollback_path")]
        public string? RollbackPath { get; set; }

        [JsonPropertyName("rest_api_bind_address")]
        public string? RestApiBindAddress { get; set; }

        [JsonPropertyName("update_server_address")]
        public string? UpdateServerAddress { get; set; }

        [JsonPropertyName("update_server_port")]
        public int UpdateServerPort { get; set; }

        [JsonPropertyName("use_authentication")]
        public bool UseAuthentication { get; set; }

        [JsonPropertyName("mqtt_topics")]
        public string[]? MqttTopics { get; set; }

        [JsonPropertyName("connect_retry_seconds")]
        public int ConnectRetrySeconds { get; set; }

        [JsonPropertyName("update_apply_timeout_seconds")]
        public int UpdateApplyTimeoutSeconds { get; set; }

        [JsonPropertyName("auth_max_retries")]
        public int AuthMaxRetries { get; set; }

        [JsonPropertyName("auto_download_updates")]
        public bool AutoDownloadUpdates { get; set; }

        [JsonPropertyName("app_is_systemd_service")]
        public bool AppIsSystemdService { get; set; }

        [JsonPropertyName("app_service_name")]
        public string? AppServiceName { get; set; }
    }

    internal record DaemonInfoResponse
    {
        [JsonPropertyName("service")]
        public string? Service { get; set; }

        [JsonPropertyName("up_time")]
        public long UpTime { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("device_info")]
        public object? DeviceInfo { get; set; }

        [JsonPropertyName("public_key")]
        public string? PublicKey { get; set; }

        [JsonPropertyName("config")]
        public DaemonConfig? Config { get; set; }
    }
}
