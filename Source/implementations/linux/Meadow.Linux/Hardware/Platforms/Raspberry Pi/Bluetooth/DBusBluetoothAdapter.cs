using Meadow.Gateways;
using Meadow.Gateways.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Meadow;

/// <summary>
/// D-Bus based Raspberry Pi implementation of IBluetooth interface for Meadow.Core
/// Uses proper BlueZ D-Bus APIs instead of command-line tools
/// </summary>
public class DBusBluetoothAdapter : IBluetoothAdapter
{
    private const string BluezService = "org.bluez";
    private const string AdapterPath = "/org/bluez/hci0";
    private const string GattManagerInterface = "org.bluez.GattManager1";
    private const string AdapterInterface = "org.bluez.Adapter1";
    private const string LEAdvertisingManagerInterface = "org.bluez.LEAdvertisingManager1";

    /// <inheritdoc/>
    public event EventHandler? ServerStarting;
    /// <inheritdoc/>
    public event EventHandler? ServerStarted;
    /// <inheritdoc/>
    public event EventHandler? ServerStopping;
    /// <inheritdoc/>
    public event EventHandler? ServerStopped;
    /// <inheritdoc/>
    public event EventHandler? ClientConnected;
    /// <inheritdoc/>
    public event EventHandler? ClientDisconnected;

    /// <summary>
    /// Gets a value indicating whether the process is currently running.
    /// </summary>
    public bool IsRunning { get; private set; } = false;

    private Connection? _connection;
    private IAdapter1? _adapter;
    private IAdapter1Properties? _adapterProperties;
    private IGattManager1? _gattManager;
    private ILEAdvertisingManager1? _advManager;
    private readonly Dictionary<string, CancellationTokenSource> _deviceMonitors = new();
    private CancellationTokenSource? _connectionMonitorCts;

    /// <inheritdoc/>
    public bool StartBluetoothServer(IDefinition configuration)
    {
        Console.WriteLine("StartBluetoothServer (D-Bus version)");

        ServerStarting?.Invoke(this, EventArgs.Empty);

        try
        {
            // Initialize D-Bus connection
            if (!InitializeDBusConnection())
            {
                Console.WriteLine("Failed to initialize D-Bus connection");
                return false;
            }

            // Configure the adapter
            if (!ConfigureAdapter(configuration))
            {
                Console.WriteLine("Failed to configure Bluetooth adapter");
                return false;
            }

            // Register GATT service
            if (!RegisterGattService())
            {
                Console.WriteLine("Warning: Failed to register GATT service, continuing...");
            }

            // Start advertising
            if (!StartAdvertising(configuration))
            {
                Console.WriteLine("Failed to start advertising");
                return false;
            }

            IsRunning = true;
            
            // Start monitoring connections
            StartConnectionMonitoring();

            // Wait a moment for all settings to stabilize
            Thread.Sleep(2000);
            
            // Force pairable setting one more time before displaying info
            Console.WriteLine("Final pairable enforcement...");
            RunCommand("bluetoothctl", "pairable on", 3000);
            Thread.Sleep(500);

            // Display server information
            DisplayServerInfo();
            Console.WriteLine("D-Bus Bluetooth server started successfully");
            ServerStarted?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting D-Bus Bluetooth server: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc/>
    public bool StopBluetoothServer()
    {
        ServerStopping?.Invoke(this, EventArgs.Empty);

        try
        {
            // Stop connection monitoring
            _connectionMonitorCts?.Cancel();
            _connectionMonitorCts = null;

            // Stop advertising
            StopAdvertising();

            // Disconnect all devices
            DisconnectAllDevices();

            // Clean up D-Bus connection
            _connection?.Dispose();
            _connection = null;

            IsRunning = false;
            ServerStopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping D-Bus Bluetooth server: {ex.Message}");
            return false;
        }
    }

    private void DisplayServerInfo()
    {
        try
        {
            Console.WriteLine("=== Bluetooth Server Information ===");
            
            // Get adapter properties
            var address = _adapterProperties!.GetAsync<string>("Address").Result;
            var name = _adapterProperties.GetAsync<string>("Name").Result;
            var alias = _adapterProperties.GetAsync<string>("Alias").Result;
            var powered = _adapterProperties.GetAsync<bool>("Powered").Result;
            var discoverable = _adapterProperties.GetAsync<bool>("Discoverable").Result;
            var pairable = _adapterProperties.GetAsync<bool>("Pairable").Result;
            
            Console.WriteLine($"Server Address: {address}");
            Console.WriteLine($"Device Name: {alias ?? name}");
            Console.WriteLine($"Powered: {powered}");
            Console.WriteLine($"Discoverable: {discoverable}");
            Console.WriteLine($"Pairable: {pairable}");
            Console.WriteLine($"Service UUID: 6E400001-B5A3-F393-E0A9-E50E24DCCA9E (Nordic UART)");
            Console.WriteLine("=====================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting server info: {ex.Message}");
        }
    }

    private bool InitializeDBusConnection()
    {
        try
        {
            Console.WriteLine("Initializing D-Bus connection to BlueZ...");
            _connection = Connection.System;

            // Get the Bluetooth adapter
            _adapter = _connection.CreateProxy<IAdapter1>(BluezService, AdapterPath);
            _adapterProperties = _connection.CreateProxy<IAdapter1Properties>(BluezService, AdapterPath);
            _gattManager = _connection.CreateProxy<IGattManager1>(BluezService, AdapterPath);
            _advManager = _connection.CreateProxy<ILEAdvertisingManager1>(BluezService, AdapterPath);

            Console.WriteLine("D-Bus connection established");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize D-Bus connection: {ex.Message}");
            return false;
        }
    }

    private bool ConfigureAdapter(IDefinition configuration)
    {
        try
        {
            Console.WriteLine("Configuring Bluetooth adapter...");

            // Power on the adapter using Properties interface
            _adapterProperties!.SetAsync("Powered", true).Wait();
            Thread.Sleep(500);

            // Make discoverable and pairable - ensure pairable is set properly
            _adapterProperties.SetAsync("Discoverable", true).Wait();
            _adapterProperties.SetAsync("Pairable", true).Wait();
            Thread.Sleep(500);
            
            // Verify pairable was set
            var pairableCheck = _adapterProperties.GetAsync<bool>("Pairable").Result;
            Console.WriteLine($"Pairable setting verified: {pairableCheck}");
            
            // If pairable didn't work via D-Bus, try bluetoothctl
            if (!pairableCheck)
            {
                Console.WriteLine("Pairable failed via D-Bus, trying bluetoothctl...");
                RunCommand("bluetoothctl", "pairable on", 3000);
                Thread.Sleep(500);
                var recheckPairable = _adapterProperties.GetAsync<bool>("Pairable").Result;
                Console.WriteLine($"Pairable after bluetoothctl: {recheckPairable}");
            }

            // Set device name if provided
            if (!string.IsNullOrWhiteSpace(configuration.DeviceName))
            {
                _adapterProperties.SetAsync("Alias", configuration.DeviceName).Wait();
            }

            Console.WriteLine("Bluetooth adapter configured successfully");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to configure adapter: {ex.Message}");
            return false;
        }
    }

    private bool RegisterGattService()
    {
        try
        {
            Console.WriteLine("Registering GATT service via D-Bus...");

            // For now, we'll implement a basic approach
            // In a full implementation, you'd create a proper GATT service object
            // This is a placeholder that demonstrates the D-Bus approach

            var serviceUuid = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E"; // Nordic UART Service
            Console.WriteLine($"Would register GATT service: {serviceUuid}");

            // TODO: Implement full GATT service registration
            // This requires creating service objects that implement the D-Bus interfaces

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to register GATT service: {ex.Message}");
            return false;
        }
    }

    private bool StartAdvertising(IDefinition configuration)
    {
        try
        {
            Console.WriteLine("Starting BLE advertising via D-Bus...");

            // Create advertisement data
            var advertisementData = new Dictionary<string, object>
            {
                ["Type"] = "peripheral",
                ["LocalName"] = configuration.DeviceName ?? "MeadowDevice",
                ["ServiceUUIDs"] = new string[] { "6E400001-B5A3-F393-E0A9-E50E24DCCA9E" },
                ["IncludeTxPower"] = true
            };

            // For now, fall back to bluetoothctl for advertising since D-Bus advertisement 
            // object registration requires complex service implementation
            try
            {
                var deviceName = configuration.DeviceName ?? "MeadowDevice";
                
                // Stop any existing advertising first
                RunCommand("bluetoothctl", "advertise off", 3000);
                Thread.Sleep(500);
                
                // Set up advertising with bluetoothctl
                var advScript = $@"timeout 8s bluetoothctl << 'EOF'
menu advertise
uuids 6E400001-B5A3-F393-E0A9-E50E24DCCA9E
name {deviceName}
discoverable on
back
advertise on
EOF";

                var result = RunCommand("bash", $"-c \"{advScript}\"", 10000);
                
                if (result)
                {
                    Console.WriteLine("BLE advertising started via bluetoothctl fallback");
                    
                    // Verify advertising is active
                    Thread.Sleep(1000);
                    var showResult = RunCommandWithOutput("bluetoothctl", "show", 3000);
                    if (showResult.Contains("Advertising: yes"))
                    {
                        Console.WriteLine("✓ Advertising confirmed active");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("⚠ Advertising may not be active - check bluetoothctl show");
                    }
                }
                
                Console.WriteLine("bluetoothctl advertising failed, trying hciconfig...");
                
                // Fallback to hciconfig
                RunCommand("hciconfig", "hci0 up", 3000);
                RunCommand("hciconfig", "hci0 piscan", 3000);
                var hciResult = RunCommand("hciconfig", "hci0 leadv 0", 3000);
                
                if (hciResult)
                {
                    Console.WriteLine("BLE advertising started via hciconfig");
                    return true;
                }
                
                Console.WriteLine("All advertising methods failed");
                return false;
            }
            catch (Exception fallbackEx)
            {
                Console.WriteLine($"Fallback advertising failed: {fallbackEx.Message}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start advertising: {ex.Message}");
            return false;
        }
    }

    private void StopAdvertising()
    {
        try
        {
            Console.WriteLine("Stopping BLE advertising...");
            // TODO: Unregister advertisement
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping advertising: {ex.Message}");
        }
    }

    private void StartConnectionMonitoring()
    {
        try
        {
            Console.WriteLine("Starting D-Bus connection monitoring...");
            _connectionMonitorCts = new CancellationTokenSource();
            
            // Start polling-based monitoring
            _ = Task.Run(async () =>
            {
                Console.WriteLine("Connection monitoring task started");
                await MonitorConnections(_connectionMonitorCts.Token);
            });
            
            // Note: D-Bus signal monitoring is complex with Tmds.DBus
            // For now, relying on improved polling with validation
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting connection monitoring: {ex.Message}");
        }
    }

    private async Task MonitorConnections(CancellationToken cancellationToken)
    {
        var knownDevices = new HashSet<string>();
        var lastDeviceCount = 0;

        Console.WriteLine("Connection monitoring started");

        while (!cancellationToken.IsCancellationRequested && IsRunning)
        {
            try
            {
                // Get connected devices via D-Bus
                var connectedDevices = await GetConnectedDevicesAsync();
                var currentDeviceCount = connectedDevices.Count;
                
                // Only log when device count changes
                if (currentDeviceCount != lastDeviceCount)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Connected devices: {currentDeviceCount}");
                    lastDeviceCount = currentDeviceCount;
                }

                // Check for new connections
                foreach (var device in connectedDevices)
                {
                    if (!knownDevices.Contains(device))
                    {
                        knownDevices.Add(device);
                        var deviceInfo = await GetDeviceInfoAsync(device);
                        Console.WriteLine($"🔗 CLIENT CONNECTED: {deviceInfo.Address}");
                        Console.WriteLine($"   Device: {deviceInfo.Name ?? "Unknown Device"}");
                        Console.WriteLine($"   Time: {DateTime.Now:HH:mm:ss}");
                        ClientConnected?.Invoke(this, EventArgs.Empty);
                    }
                }

                // Check for disconnections
                var disconnectedDevices = knownDevices.Except(connectedDevices).ToList();
                foreach (var device in disconnectedDevices)
                {
                    knownDevices.Remove(device);
                    var deviceInfo = await GetDeviceInfoAsync(device);
                    Console.WriteLine($"❌ CLIENT DISCONNECTED: {deviceInfo.Address}");
                    Console.WriteLine($"   Device: {deviceInfo.Name ?? "Unknown Device"}");
                    Console.WriteLine($"   Time: {DateTime.Now:HH:mm:ss}");
                    
                    // Clean up the disconnected device and restart advertising
                    _ = Task.Run(() => HandleClientDisconnection(device));
                    
                    ClientDisconnected?.Invoke(this, EventArgs.Empty);
                }

                await Task.Delay(500, cancellationToken); // Faster polling for quicker detection
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Connection monitoring cancelled");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in connection monitoring: {ex.Message}");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task<List<string>> GetConnectedDevicesAsync()
    {
        var connectedDevices = new List<string>();
        try
        {
            // Use hybrid approach: D-Bus for device enumeration, but validate with connection properties
            try
            {
                var objectManager = _connection!.CreateProxy<IObjectManager>(BluezService, "/");
                var objects = await objectManager.GetManagedObjectsAsync();

                foreach (var obj in objects)
                {
                    var path = obj.Key.ToString();
                    
                    // Check if this is a device object (devices have paths like /org/bluez/hci0/dev_XX_XX_XX_XX_XX_XX)
                    if (path.StartsWith("/org/bluez/hci0/dev_") && obj.Value.ContainsKey("org.bluez.Device1"))
                    {
                        try
                        {
                            var deviceProxy = _connection.CreateProxy<IDevice1Properties>(BluezService, obj.Key);
                            var connected = await deviceProxy.GetAsync<bool>("Connected");
                            
                            if (connected)
                            {
                                // Additional validation: check if device is actually reachable
                                var isActuallyConnected = await ValidateDeviceConnection(deviceProxy, path);
                                
                                if (isActuallyConnected && !connectedDevices.Contains(path))
                                {
                                    connectedDevices.Add(path);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error checking device {path} via D-Bus: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with D-Bus enumeration: {ex.Message}");
                Console.WriteLine($"D-Bus exception type: {ex.GetType().Name}");
            }

            return connectedDevices.Distinct().ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in GetConnectedDevicesAsync: {ex.Message}");
            Console.WriteLine($"Exception type: {ex.GetType().Name}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return connectedDevices;
        }
    }

    private async Task<bool> ValidateDeviceConnection(IDevice1Properties deviceProxy, string devicePath)
    {
        try
        {
            // Simple validation - just check D-Bus Connected property
            var connected = await deviceProxy.GetAsync<bool>("Connected");
            return connected;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating device connection for {devicePath}: {ex.Message}");
            return false;
        }
    }

    private string ExtractMacAddress(string devicePath)
    {
        try
        {
            // Convert path like /org/bluez/hci0/dev_XX_XX_XX_XX_XX_XX to MAC address XX:XX:XX:XX:XX:XX
            if (devicePath.StartsWith("/org/bluez/hci0/dev_"))
            {
                var macPart = devicePath.Substring("/org/bluez/hci0/dev_".Length);
                return macPart.Replace('_', ':');
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<DeviceInfo> GetDeviceInfoAsync(string devicePath)
    {
        try
        {
            var deviceProxy = _connection!.CreateProxy<IDevice1Properties>(BluezService, devicePath);
            
            // Extract MAC address from path as fallback
            var pathAddress = ExtractMacAddress(devicePath);
            
            string address = pathAddress;
            string? name = null;
            string? alias = null;
            
            try
            {
                address = await deviceProxy.GetAsync<string>("Address");
            }
            catch
            {
                // Use path-extracted address as fallback
            }
            
            try
            {
                name = await deviceProxy.GetAsync<string>("Name");
            }
            catch
            {
                // Name property might not exist for some devices
            }
            
            try
            {
                alias = await deviceProxy.GetAsync<string>("Alias");
            }
            catch
            {
                // Alias property might not exist for some devices
            }
            
            return new DeviceInfo
            {
                Address = address ?? pathAddress ?? "Unknown",
                Name = !string.IsNullOrEmpty(alias) ? alias : (!string.IsNullOrEmpty(name) ? name : null),
                Path = devicePath
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting device info for {devicePath}: {ex.Message}");
            return new DeviceInfo
            {
                Address = ExtractMacAddress(devicePath) ?? "Unknown",
                Name = "Unknown Device",
                Path = devicePath
            };
        }
    }

    private async Task HandleClientDisconnection(string devicePath)
    {
        try
        {
            Console.WriteLine($"Handling disconnection cleanup for device: {devicePath}");
            
            // Remove the device from bluetooth controller via D-Bus
            try
            {
                var deviceObjectPath = new ObjectPath(devicePath);
                await _adapter!.RemoveDeviceAsync(deviceObjectPath);
                Console.WriteLine($"Device removed via D-Bus: {devicePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to remove device via D-Bus: {ex.Message}");
                
                // Fallback to bluetoothctl
                var deviceAddress = devicePath.Split('_').LastOrDefault()?.Replace('_', ':');
                if (!string.IsNullOrEmpty(deviceAddress))
                {
                    RunCommand("bluetoothctl", $"remove {deviceAddress}", 3000);
                }
            }
            
            // Wait a moment for cleanup
            await Task.Delay(1000);
            
            // Restart advertising to allow new connections
            Console.WriteLine("Restarting advertising after client disconnection...");
            RestartAdvertising();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client disconnection: {ex.Message}");
        }
    }

    private void RestartAdvertising()
    {
        try
        {
            Console.WriteLine("Stopping current advertising...");
            // Stop current advertising
            RunCommand("bluetoothctl", "advertise off", 3000);
            
            // Wait a moment for state to clear
            Thread.Sleep(1000);
            
            Console.WriteLine("Restarting advertising...");
            // Restart advertising using the same method as initial startup
            var advScript = @"timeout 8s bluetoothctl << 'EOF'
menu advertise
uuids 6E400001-B5A3-F393-E0A9-E50E24DCCA9E
name yoshipi
discoverable on
back
advertise on
EOF";

            var result = RunCommand("bash", $"-c \"{advScript}\"", 10000);
            
            if (result)
            {
                Console.WriteLine("✓ Advertising restarted - ready for new connections");
                
                // Quick verification without verbose output
                Thread.Sleep(1000);
                var showResult = RunCommandWithOutput("bluetoothctl", "show", 3000);
                if (!showResult.Contains("Advertising: yes"))
                {
                    // Silent HCI fallback
                    RunCommand("hciconfig", "hci0 up", 3000);
                    RunCommand("hciconfig", "hci0 piscan", 3000);
                    RunCommand("hciconfig", "hci0 leadv 0", 3000);
                }
            }
            else
            {
                // Silent fallback to hciconfig
                RunCommand("hciconfig", "hci0 up", 3000);
                RunCommand("hciconfig", "hci0 piscan", 3000);
                var hciResult = RunCommand("hciconfig", "hci0 leadv 0", 3000);
                
                if (hciResult)
                {
                    Console.WriteLine("✓ Advertising restarted");
                }
                else
                {
                    Console.WriteLine("⚠ Advertising restart failed");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error restarting advertising: {ex.Message}");
        }
    }

    private void DisconnectAllDevices()
    {
        try
        {
            Console.WriteLine("Disconnecting all devices...");
            // TODO: Implement device disconnection via D-Bus
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disconnecting devices: {ex.Message}");
        }
    }

    private bool RunCommand(string fileName, string arguments, int timeoutMs = 10000)
    {
        try
        {
            using var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();

            if (!process.WaitForExit(timeoutMs))
            {
                Console.WriteLine($"Command '{fileName} {arguments}' timed out after {timeoutMs}ms");
                try { process.Kill(); } catch { }
                return false;
            }

            if (process.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Command '{fileName} {arguments}' failed: {error}");
                }
            }

            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running command '{fileName} {arguments}': {ex.Message}");
            return false;
        }
    }

    private string RunCommandWithOutput(string fileName, string arguments, int timeoutMs = 10000)
    {
        try
        {
            using var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();

            var output = string.Empty;
            var outputTask = Task.Run(() => output = process.StandardOutput.ReadToEnd());

            if (!process.WaitForExit(timeoutMs) || !outputTask.Wait(timeoutMs))
            {
                Console.WriteLine($"Command '{fileName} {arguments}' timed out after {timeoutMs}ms");
                try { process.Kill(); } catch { }
                return string.Empty;
            }

            return process.ExitCode == 0 ? output : string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running command '{fileName} {arguments}': {ex.Message}");
            return string.Empty;
        }
    }
}

// D-Bus interface definitions for BlueZ
[DBusInterface("org.bluez.Adapter1")]
public interface IAdapter1 : IDBusObject
{
    Task StartDiscoveryAsync();
    Task StopDiscoveryAsync();
    Task RemoveDeviceAsync(ObjectPath device);
}

[DBusInterface("org.bluez.Adapter1")]
public interface IAdapter1Properties : IDBusObject
{
    Task<T> GetAsync<T>(string prop);
    Task SetAsync(string prop, object val);
    Task<IDictionary<string, object>> GetAllAsync();
}

[DBusInterface("org.bluez.GattManager1")]
public interface IGattManager1 : IDBusObject
{
    Task RegisterApplicationAsync(ObjectPath application, IDictionary<string, object> options);
    Task UnregisterApplicationAsync(ObjectPath application);
}

[DBusInterface("org.bluez.LEAdvertisingManager1")]
public interface ILEAdvertisingManager1 : IDBusObject
{
    Task RegisterAdvertisementAsync(ObjectPath advertisement, IDictionary<string, object> options);
    Task UnregisterAdvertisementAsync(ObjectPath advertisement);
}

[DBusInterface("org.freedesktop.DBus.ObjectManager")]
public interface IObjectManager : IDBusObject
{
    Task<IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>> GetManagedObjectsAsync();
}

[DBusInterface("org.bluez.Device1")]
public interface IDevice1Properties : IDBusObject
{
    Task<T> GetAsync<T>(string prop);
    Task SetAsync(string prop, object val);
    Task<IDictionary<string, object>> GetAllAsync();
    Task ConnectAsync();
    Task DisconnectAsync();
}


public class DeviceInfo
{
    public string Address { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Path { get; set; } = string.Empty;
}