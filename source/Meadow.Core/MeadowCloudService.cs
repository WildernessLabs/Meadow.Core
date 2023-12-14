using Meadow.Cloud;
using Meadow.Update;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SRI = System.Runtime.InteropServices;

namespace Meadow;

/// <summary>
/// Encapsulates logic for communicating with Meadow.Cloud
/// </summary>
public partial class MeadowCloudService : IMeadowCloudService
{
    /// <inheritdoc/>
    public event EventHandler<string> ServiceError = default!;

    /// <summary>
    /// Creates a MeadowCloudService
    /// </summary>
    /// <param name="settings">The settings used for interacting with the service</param>
    public MeadowCloudService(IMeadowCloudSettings settings)
    {
        Settings = settings;
    }

    /// <summary>
    /// Gets or sets the cloud service settings
    /// </summary>
    public IMeadowCloudSettings Settings { get; protected set; }

    /// <inheritdoc/>
    public string? CurrentJwt { get; protected set; }

    /// <inheritdoc/>
    public async Task<bool> Authenticate()
    {
        string errorMessage;

        using (var client = new HttpClient())
        {
            client.Timeout = new TimeSpan(0, 15, 0);

            var json = JsonSerializer.Serialize<dynamic>(new { id = Resolver.Device.Information.UniqueID.ToUpper() });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = $"{Settings.Hostname}/api/devices/login";
            Resolver.Log.Debug($"Attempting to login to {endpoint} with {json}...");

            var response = await client.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Resolver.Log.Debug($"authentication successful. extracting token");
                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                var payload = JsonSerializer.Deserialize<MeadowCloudLoginResponseMessage>(responseContent, opts);

                if (payload == null)
                {
                    Resolver.Log.Warn($"invalid auth payload");
                    CurrentJwt = null;
                    return false;
                }

                Resolver.Log.Debug($"decrypting auth payload");
                var encryptedKeyBytes = System.Convert.FromBase64String(payload.EncryptedKey);

                byte[]? decryptedKey;
                try
                {
                    var privateKey = GetPrivateKeyInPemFormat();
                    if (privateKey == null)
                    {
                        return false;
                    }

                    decryptedKey = Resolver.Device.PlatformOS.RsaDecrypt(encryptedKeyBytes, privateKey);
                }
                catch (OverflowException)
                {
                    // dev note: bug in pre-0.9.6.3 on F7 will provision with a bad key and end up here
                    // TODO: add platform and OS checking for this?
                    errorMessage = $"RSA decrypt failure. This device likely needs to be reprovisioned.";
                    Resolver.Log.Error(errorMessage);
                    ServiceError?.Invoke(this, errorMessage);

                    CurrentJwt = null;
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = $"RSA decrypt failure: {ex.Message}";
                    Resolver.Log.Error(errorMessage);
                    ServiceError?.Invoke(this, errorMessage);

                    CurrentJwt = null;
                    return false;
                }

                // then need to call method to AES decrypt the EncryptedToken with IV
                try
                {
                    var encryptedTokenBytes = Convert.FromBase64String(payload.EncryptedToken);
                    var ivBytes = Convert.FromBase64String(payload.Iv);
                    var decryptedToken = Resolver.Device.PlatformOS.AesDecrypt(encryptedTokenBytes, decryptedKey, ivBytes);

                    CurrentJwt = Encoding.UTF8.GetString(decryptedToken);

                    // trim any "unprintable character" padding.  in my testing it was a 0x05, but unsure if that's consistent, so this is safer
                    CurrentJwt = Regex.Replace(CurrentJwt, @"[^\w\.@-]", "");

                    Resolver.Log.Debug($"auth token successfully received");
                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = $"AES decrypt failure: {ex.Message}";
                    Resolver.Log.Error(errorMessage);
                    ServiceError?.Invoke(this, errorMessage);

                    CurrentJwt = null;
                    return false;
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // device is likely not provisioned?
                errorMessage = $"Update service returned 'Not Found': this device has likely not been provisioned";
            }
            else
            {
                errorMessage = $"Update service login returned {response.StatusCode}: {responseContent}";
            }

            Resolver.Log.Warn(errorMessage);
            ServiceError?.Invoke(this, errorMessage);

            CurrentJwt = null;
            return false;
        }
    }

    /// <inheritdoc/>
    public Task<bool> SendLog(CloudLog log)
    {
        return Send(log, "/api/logs");
    }

    /// <inheritdoc/>
    public Task<bool> SendEvent(CloudEvent cloudEvent)
    {
        return Send(cloudEvent, "/api/events");
    }

    private async Task<bool> Send<T>(T item, string endpoint)
    {
        int attempt = 0;
        int maxRetries = 1;
        var result = false;
        string errorMessage;

    retry:

        if (CurrentJwt == null)
        {
            if (!await Authenticate())
            {
                errorMessage = $"Cloud auth failed before sending";
                ServiceError?.Invoke(this, errorMessage);
                Resolver.Log.Warn(errorMessage);
                // TODO: should we retry after this?
                return false;
            }
        }

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(Settings.DataHostname);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentJwt);

            var serializeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(item, serializeOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Resolver.Log.Debug($"making cloud log httprequest with json: {json}");

            HttpResponseMessage response;

            try
            {
                response = await client.PostAsync($"{endpoint}", content);
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to send Meadow.Cloud data: {ex.Message}";
                Resolver.Log.Warn(errorMessage);
                ServiceError?.Invoke(this, errorMessage);

                return false;
            }

            if (response.IsSuccessStatusCode)
            {
                Resolver.Log.Debug("cloud send success");
                result = true;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (attempt < maxRetries)
                {
                    attempt++;
                    Resolver.Log.Debug($"Unauthorized, re-authenticating");
                    // by setting this to null and retrying, Authenticate will get called
                    CurrentJwt = null;
                    goto retry;
                }
                else
                {
                    errorMessage = $"Unauthorized, exceeded {maxRetries} retry attempt(s)";
                    Resolver.Log.Warn(errorMessage);
                    ServiceError?.Invoke(this, errorMessage);

                    result = false;
                }
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                errorMessage = $"Cloud send failed. Reponse was '{responseContent}'";
                Resolver.Log.Warn(errorMessage);
                ServiceError?.Invoke(this, errorMessage);

                result = false;
            }
        }

        //ToDo remove
        GC.Collect();
        return result;
    }

    private string? GetPrivateKeyInPemFormat()
    {
        // DEV NOTE: I don't want to expose a "GetPrivateKey()" method in an interface, or anything non-private. That's why this is like it is
        if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetWindowsPrivateKeyInPemFormat();
        }
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return GetMacPrivateKeyInPemFormat();
        }
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetLinuxPrivateKeyInPemFormat();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    private string ExecuteBashCommandLine(string command)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        process?.WaitForExit();

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }
}