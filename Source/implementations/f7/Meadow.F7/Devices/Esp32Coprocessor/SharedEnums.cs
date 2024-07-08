namespace Meadow.Devices.Esp32.MessagePayloads;

/// <summary>
/// Status and error codes for the various function calls.
/// </summary>
public enum StatusCodes
{
    /// <summary>
    /// StatusCodes - CompletedOk
    /// </summary>
    CompletedOk = 0,
    /// <summary>
    /// StatusCodes - CrcError
    /// </summary>
    CrcError = 1,
    /// <summary>
    /// StatusCodes - Restart
    /// </summary>
    Restart = 2,
    /// <summary>
    /// StatusCodes - Failure
    /// </summary>
    Failure = 3,
    /// <summary>
    /// StatusCodes - InvalidInterface
    /// </summary>
    InvalidInterface = 4,
    /// <summary>
    /// StatusCodes - QueueError
    /// </summary>
    QueueError = 5,
    /// <summary>
    /// StatusCodes - Timeout
    /// </summary>
    Timeout = 6,
    /// <summary>
    /// StatusCodes - InvalidPacket
    /// </summary>
    InvalidPacket = 7,
    /// <summary>
    /// StatusCodes - InvalidHeader
    /// </summary>
    InvalidHeader = 8,
    /// <summary>
    /// StatusCodes - UnexpectedData
    /// </summary>
    UnexpectedData = 9,
    /// <summary>
    /// StatusCodes - MissingEndOfFrameMarker
    /// </summary>
    MissingEndOfFrameMarker = 10,
    /// <summary>
    /// StatusCodes - HeaderBodyFieldMismatch
    /// </summary>
    HeaderBodyFieldMismatch = 11,
    /// <summary>
    /// StatusCodes - WiFiAlreadyStarted
    /// </summary>
    WiFiAlreadyStarted = 12,
    /// <summary>
    /// StatusCodes - InvalidWiFiCredentials
    /// </summary>
    InvalidWiFiCredentials = 13,
    /// <summary>
    /// StatusCodes - WiFiDisconnected
    /// </summary>
    WiFiDisconnected = 14,
    /// <summary>
    /// StatusCodes - CannotStartNetworkInterface
    /// </summary>
    CannotStartNetworkInterface = 15,
    /// <summary>
    /// StatusCodes - CannotConnectToAccessPoint
    /// </summary>
    CannotConnectToAccessPoint = 16,
    /// <summary>
    /// StatusCodes - DefaultAccessPointNotConfigured
    /// </summary>
    DefaultAccessPointNotConfigured = 17,
    /// <summary>
    /// StatusCodes - InvalidAntennaData
    /// </summary>
    InvalidAntennaData = 18,
    /// <summary>
    /// StatusCodes - InvalidAntennaValue
    /// </summary>
    InvalidAntennaValue = 19,
    /// <summary>
    /// StatusCodes - InvalidIp
    /// </summary>
    InvalidIp = 20,
    /// <summary>
    /// StatusCodes - NoMessagesWaiting
    /// </summary>
    NoMessagesWaiting = 21,
    /// <summary>
    /// StatusCodes - CoprocessorNotResponding
    /// </summary>
    CoprocessorNotResponding = 22,
    /// <summary>
    /// StatusCodes - EspWiFiNotStarted
    /// </summary>
    EspWiFiNotStarted = 23,
    /// <summary>
    /// StatusCodes - EspOutOfMemory
    /// </summary>
    EspOutOfMemory = 24,
    /// <summary>
    /// StatusCodes - EspWiFiInvalidSsid
    /// </summary>
    EspWiFiInvalidSsid = 25,
    /// <summary>
    /// StatusCodes - AccessPointNotFound
    /// </summary>
    AccessPointNotFound = 26,
    /// <summary>
    /// StatusCodes - BeaconTimeout
    /// </summary>
    BeaconTimeout = 27,
    /// <summary>
    /// StatusCodes - AuthenticationFailed
    /// </summary>
    AuthenticationFailed = 28,
    /// <summary>
    /// StatusCodes - AssociationFailed
    /// </summary>
    AssociationFailed = 29,
    /// <summary>
    /// StatusCodes - HandshakeTimeout
    /// </summary>
    HandshakeTimeout = 30,
    /// <summary>
    /// StatusCodes - ConnectionFailed
    /// </summary>
    ConnectionFailed = 31,
    /// <summary>
    /// StatusCodes - ApTsfReset
    /// </summary>
    ApTsfReset = 32,
    /// <summary>
    /// StatusCodes - UnmappedErrorCode
    /// </summary>
    UnmappedErrorCode = 33,
    /// <summary>
    /// StatusCodes - UnknownConfigurationItem
    /// </summary>
    UnknownConfigurationItem = 34,
    /// <summary>
    /// StatusCodes - CannotStartAccessPoint
    /// </summary>
    CannotStartAccessPoint = 35,
    /// <summary>
    /// StatusCodes - DhcpConfigurationError
    /// </summary>
    DhcpConfigurationError = 36,
    /// <summary>
    /// StatusCodes - AccessPointNotStarted
    /// </summary>
    AccessPointNotStarted = 37,
    /// <summary>
    /// StatusCodes - AccessPointAlreadyStarted
    /// </summary>
    AccessPointAlreadyStarted = 38,
    /// <summary>
    /// StatusCodes - NotImplemented
    /// </summary>
    NotImplemented = 39,
    /// <summary>
    /// File could not be found on the ESP file system
    /// </summary>
    FileNotFound = 40,
    /// <summary>
    /// Threadpool on the ESP32 is full.
    /// </summary>
    ThreadPoolIsFull = 41,
    /// <summary>
    /// ESP32 has restarted unexpectedly.
    /// </summary>
    UnexpectedCoprocessorRestart = 42,
    /// <summary>
    /// Configuration contains one or more errors.
    /// </summary>
    InvalidConfigurationFile = 43,
    /// <summary>
    /// WiFi configuration contains one or more errors.
    /// </summary>
    InvalidWiFiConfigurationFile = 44,
    /// <summary>
    /// Cell configuration contains one or more errors.
    /// </summary>
    InvalidCellConfigurationFile = 45,
    /// <summary>
    /// Network deadlock detected.
    /// </summary>
    NetworkDeadlock = 46,
    /// <summary>
    /// ESP reset for unknown reason
    /// </summary>
    EspReset = 50,
    /// <summary>
    /// Power-on or EN line pulled low and released.
    /// </summary>
    EspResetPowerOn = 51,
    /// <summary>
    /// Reset using an external GPIO (n/a for the ESP32).
    /// </summary>
    EspResetExternalGpio = 52,
    /// <summary>
    /// `esp_restart` has used to reset the ESP32.
    /// </summary>
    EspResetSoftware = 53,
    /// <summary>
    /// Reset due to software exception or panic.
    /// </summary>
    EspResetPanic = 54,
    /// <summary>
    /// Reset due to interrupt watchdog.
    /// </summary>
    EspResetInterruptWatchdog = 55,
    /// <summary>
    /// Reset due to task watchdog.
    /// </summary>
    EspResetTaskWatchdog = 56,
    /// <summary>
    /// Reset due to watchdog other than task or interrupt watchdogs.
    /// </summary>
    EspResetOtherWatchdog = 57,
    /// <summary>
    /// Reset after exiting deep sleep.
    /// </summary>
    EspResetDeepSleep = 58,
    /// <summary>
    /// Hardware or software brownout reset.
    /// </summary>
    EspResetBrownout = 59,
    /// <summary>
    /// Reset over SDIO.
    /// </summary>
    EspResetSDIO = 60,
}

/// <summary>
/// System interfaces available on the ESP32 for the various function calls.
/// </summary>
public enum Esp32Interfaces
{
    /// <summary>
    /// Esp32Interfaces - None
    /// </summary>
    None = 0,
    /// <summary>
    /// Esp32Interfaces - WiFi
    /// </summary>
    WiFi = 1,
    /// <summary>
    /// Esp32Interfaces - BlueTooth
    /// </summary>
    BlueTooth = 2,
    /// <summary>
    /// Esp32Interfaces - MeshNetwork
    /// </summary>
    MeshNetwork = 3,
    /// <summary>
    /// Esp32Interfaces - System
    /// </summary>
    System = 4,
    /// <summary>
    /// Esp32Interfaces - Transport
    /// </summary>
    Transport = 5,
    /// <summary>
    /// Esp32Interfaces - WiredEthernet
    /// </summary>
    WiredEthernet = 6,
    /// <summary>
    /// Esp32Interfaces - Cell
    /// </summary>
    Cell = 7
};

/// <summary>
/// System functions available on the ESP32
/// </summary>
public enum SystemFunction
{
    /// <summary>
    /// SystemFunction - GetConfiguration
    /// </summary>
    GetConfiguration = 0,
    /// <summary>
    /// SystemFunction - SetConfigurationItem
    /// </summary>
    SetConfigurationItem = 1,
    /// <summary>
    /// SystemFunction - DeepSleep
    /// </summary>
    DeepSleep = 2,
    /// <summary>
    /// SystemFunction - GetBatteryChargeLevel
    /// </summary>
    GetBatteryChargeLevel = 3,
    /// <summary>
    /// SystemFunction - ErrorEvent
    /// </summary>
    ErrorEvent = 4,
    /// <summary>
    /// SystemFunction - StartHeapTrace
    /// </summary>
    StartHeapTrace = 5,
    /// <summary>
    /// SystemFunction - StopHeapTrace
    /// </summary>
    StopHeapTrace = 6,

    //
    //  System functions 7 - 11 inclusive are not exposed to managed code.
    //

    /// <summary>
    /// SystemFunction - Operating system exception
    /// </summary>
    OsException = 12
};

/// <summary>
/// WiFi functions available on the ESP32
/// </summary>
internal enum WiFiFunction
{
    // DEV NOTE:
    // THESE COME FROM espcp_shared_enums.h, which can be found here:
    // https://github.com/WildernessLabs/Meadow/blob/main/nuttx/configs/stm32f777zit6-meadow/src/espcp/espcp_shared_enums.h

    /// <summary>
    /// WiFiFunction - StartWiFiInterface
    /// </summary>
    StartWiFiInterface = 0,
    /// <summary>
    /// WiFiFunction - StopWiFiInterface
    /// </summary>
    StopWiFiInterface = 1,
    /// <summary>
    /// WiFiFunction - ConnectToAccessPoint
    /// </summary>
    ConnectToAccessPoint = 2,
    /// <summary>
    /// WiFiFunction - ConnectToDefaultAccessPoint
    /// </summary>
    ConnectToDefaultAccessPoint = 3,
    /// <summary>
    /// WiFiFunction - ClearDefaultAccessPoint
    /// </summary>
    ClearDefaultAccessPoint = 4,
    /// <summary>
    /// WiFiFunction - DisconnectFromAccessPoint
    /// </summary>
    DisconnectFromAccessPoint = 5,
    /// <summary>
    /// WiFiFunction - GetAccessPoints
    /// </summary>
    GetAccessPoints = 6,
    /// <summary>
    /// WiFiFunction - SetAntenna
    /// </summary>
    SetAntenna = 7,
    /// <summary>
    /// WiFiFunction - Socket
    /// </summary>
    Socket = 8,
    /// <summary>
    /// WiFiFunction - Connect
    /// </summary>
    Connect = 9,
    /// <summary>
    /// WiFiFunction - Write
    /// </summary>
    Write = 10,
    /// <summary>
    /// WiFiFunction - SetSockOpt
    /// </summary>
    SetSockOpt = 11,
    /// <summary>
    /// WiFiFunction - GetSockOpt
    /// </summary>
    GetSockOpt = 12,
    /// <summary>
    /// WiFiFunction - Read
    /// </summary>
    Read = 13,
    /// <summary>
    /// WiFiFunction - Close
    /// </summary>
    Close = 14,
    /// <summary>
    /// WiFiFunction - SendTo
    /// </summary>
    SendTo = 15,
    /// <summary>
    /// WiFiFunction - RecvFrom
    /// </summary>
    RecvFrom = 16,
    /// <summary>
    /// WiFiFunction - Poll
    /// </summary>
    Poll = 17,
    /// <summary>
    /// WiFiFunction - InterruptPollResponse
    /// </summary>
    InterruptPollResponse = 18,
    /// <summary>
    /// WiFiFunction - Send
    /// </summary>
    Send = 19,
    /// <summary>
    /// WiFiFunction - Bind
    /// </summary>
    Bind = 20,
    /// <summary>
    /// WiFiFunction - Listen
    /// </summary>
    Listen = 21,
    /// <summary>
    /// WiFiFunction - Accept
    /// </summary>
    Accept = 22,
    /// <summary>
    /// WiFiFunction - Ioctl
    /// </summary>
    Ioctl = 23,
    /// <summary>
    /// WiFiFunction - GetSockName
    /// </summary>
    GetSockName = 24,
    /// <summary>
    /// WiFiFunction - GetPeerName
    /// </summary>
    GetPeerName = 25,
    /// <summary>
    /// WiFiFunction - FreeAddrInfo
    /// </summary>
    FreeAddrInfo = 26,
    /// <summary>
    /// WiFiFunction - GetAddrInfo
    /// </summary>
    GetAddrInfo = 27,
    /// <summary>
    /// WiFiFunction - RecvMsg
    /// </summary>
    RecvMsg = 28,
    /// <summary>
    /// WiFiFunction - Shutdown
    /// </summary>
    Shutdown = 29,
    /// <summary>
    /// WiFiFunction - SendMsg
    /// </summary>
    SendMsg = 30,
    /// <summary>
    /// WiFiFunction - Dup2
    /// </summary>
    Dup2 = 31,
    /// <summary>
    /// WiFiFunction - AddRef
    /// </summary>
    AddRef = 32,
    /// <summary>
    /// WiFiFunction - SockCaps
    /// </summary>
    SockCaps = 33,
    /// <summary>
    /// WiFiFunction - StartWiFiInterfaceEvent
    /// </summary>
    StartWiFiInterfaceEvent = 34,
    /// <summary>
    /// WiFiFunction - StopWiFiInterfaceEvent
    /// </summary>
    StopWiFiInterfaceEvent = 35,
    /// <summary>
    /// WiFiFunction - NetworkConnectedEvent
    /// </summary>
    NetworkConnectedEvent = 36,
    /// <summary>
    /// WiFiFunction - NetworkDisconnectedEvent
    /// </summary>
    NetworkDisconnectedEvent = 37,
    /// <summary>
    /// WiFiFunction - NtpUpdateEvent
    /// </summary>
    NtpUpdateEvent = 38,
    /// <summary>
    /// WiFiFunction - ErrorEvent
    /// </summary>
    ErrorEvent = 39,
    /// <summary>
    /// WiFiFunction - StartAccessPoint
    /// </summary>
    StartAccessPoint = 40,
    /// <summary>
    /// WiFiFunction - StopAccessPoint
    /// </summary>
    StopAccessPoint = 41,
    /// <summary>
    /// WiFiFunction - AccessPointStartedEvent
    /// </summary>
    AccessPointStartedEvent = 42,
    /// <summary>
    /// WiFiFunction - AccessPointStoppedEvent
    /// </summary>
    AccessPointStoppedEvent = 43,
    /// <summary>
    /// WiFiFunction - NodeConnectedEvent
    /// </summary>
    NodeConnectedEvent = 44,
    /// <summary>
    /// WiFiFunction - NodeDisconnectedEvent
    /// </summary>
    NodeDisconnectedEvent = 45,
    /// <summary>
    /// WiFiFunction - ConnectionRetryCountExceeded
    /// </summary>
    ConnectionRetryCountExceeded = 46,
    /// <summary>
    /// WiFiFunction - NetworkConnecting
    /// </summary>
    NetworkConnecting = 47,
};

/// <summary>
/// Bluetooth functions available on the ESP32
/// </summary>
public enum BluetoothFunction
{
    /// <summary>
    /// BluetoothFunction - Start
    /// </summary>
    Start = 0,
    /// <summary>
    /// BluetoothFunction - Stop
    /// </summary>
    Stop = 1,
    /// <summary>
    /// BluetoothFunction - GetHandles
    /// </summary>
    GetHandles = 2,
    /// <summary>
    /// BluetoothFunction - ServerDataSet
    /// </summary>
    ServerDataSet = 3,
    /// <summary>
    /// BluetoothFunction - ClientWriteRequestEvent
    /// </summary>
    ClientWriteRequestEvent = 4
};

/// <summary>
/// Cell functions available on the ESP32
/// </summary>
public enum CellFunction
{
    /// <summary>
    /// CellFunction - NetworkConnectedEvent
    /// </summary>
    NetworkConnectedEvent = 0,
    /// <summary>
    /// CellFunction - NetworkDisconnectedEvent
    /// </summary>
    NetworkDisconnectedEvent = 1,
    /// <summary>
    /// CellFunction - NetworkErrorEvent
    /// </summary>
    NetworkErrorEvent = 2,
    /// <summary>
    /// CellFunction - NetworkAtCmdEvent
    /// </summary>
    NetworkAtCmdEvent = 4,
    /// <summary>
    /// CellFunction - NtpUpdateEvent
    /// </summary>
    NtpUpdateEvent = 5,
};

/// <summary>
/// Ethernet functions available on the ESP32
/// </summary>
public enum EthernetFunction
{
    /// <summary>
    /// EthernetFunction - NetworkConnectedEvent
    /// </summary>
    NetworkConnectedEvent = 36,
    /// <summary>
    /// EthernetFunction - NetworkDisconnectedEvent
    /// </summary>
    NetworkDisconnectedEvent = 37,
    /// <summary>
    /// EthernetFunction - NtpUpdateEvent
    /// </summary>
    NtpUpdateEvent = 38,
};

/// <summary>
/// Enumeration representing possible cell-related errors.
/// </summary>
public enum CellError
{
    /// <summary>
    /// CellError - InvalidNetworkSettings
    /// </summary>
    InvalidNetworkSettings = 0,
    /// <summary>
    /// CellError - InvalidCellModule
    /// </summary>
    InvalidCellModule = 1,
    /// <summary>
    /// CellError - NetworkConnectionLost
    /// </summary>
    NetworkConnectionLost = 2,
    /// <summary>
    /// CellError - NetworkTimeout
    /// </summary>
    NetworkTimeout = 3,
};

/// <summary>
/// Transport functions available on the ESP32
/// </summary>
public enum TransportFunction
{
    /// <summary>
    /// TransportFunction - ResponseReady
    /// </summary>
    ResponseReady = 0,
    /// <summary>
    /// TransportFunction - SendResponse
    /// </summary>
    SendResponse = 1,
    /// <summary>
    /// TransportFunction - KillNuttxThread
    /// </summary>
    KillNuttxThread = 2,
    /// <summary>
    /// TransportFunction - ResetEsp32
    /// </summary>
    ResetEsp32 = 3
};

/// <summary>
/// Possible transport packet types.
/// </summary>
public enum MessageTypes
{
    /// <summary>
    /// MessageTypes - Ack
    /// </summary>
    Ack = 0x00,
    /// <summary>
    /// MessageTypes - Nak
    /// </summary>
    Nak = 0x01,
    /// <summary>
    /// MessageTypes - Reset
    /// </summary>
    Reset = 0x02,
    /// <summary>
    /// MessageTypes - Event
    /// </summary>
    Event = 0x04,
    /// <summary>
    /// MessageTypes - Response
    /// </summary>
    Response = 0x10,
    /// <summary>
    /// MessageTypes - Transport
    /// </summary>
    Transport = 0x20,
    /// <summary>
    /// MessageTypes - Header
    /// </summary>
    Header = 0x40,
    /// <summary>
    /// MessageTypes - Data
    /// </summary>
    Data = 0x80
};

/// <summary>
/// Name of items that can be configured (changed by the code on the STM32) on the ESP32.
/// </summary>
public enum ConfigurationItems
{
    /// <summary>
    /// ConfigurationItems - MaximumMessageQueueLength
    /// </summary>
    MaximumMessageQueueLength = 0,
    /// <summary>
    /// ConfigurationItems - AutomaticallyStartNetwork
    /// </summary>
    AutomaticallyStartNetwork = 1,
    /// <summary>
    /// ConfigurationItems - AutomaticallyReconnect
    /// </summary>
    AutomaticallyReconnect = 2,
    /// <summary>
    /// ConfigurationItems - MaximumRetryCount
    /// </summary>
    MaximumRetryCount = 3,
    /// <summary>
    /// ConfigurationItems - DeviceName
    /// </summary>
    DeviceName = 4,
    /// <summary>
    /// ConfigurationItems - DefaultApAndPassword
    /// </summary>
    DefaultApAndPassword = 5,
    /// <summary>
    /// ConfigurationItems - NtpServer
    /// </summary>
    NtpServer = 6,
    /// <summary>
    /// ConfigurationItems - GetTimeAtStartup
    /// </summary>
    GetTimeAtStartup = 7,
    /// <summary>
    /// ConfigurationItems - UseDhcp
    /// </summary>
    UseDhcp = 8,
    /// <summary>
    /// ConfigurationItems - StaticIpAddress
    /// </summary>
    StaticIpAddress = 9,
    /// <summary>
    /// ConfigurationItems - DnsServer
    /// </summary>
    DnsServer = 10,
    /// <summary>
    /// ConfigurationItems - DefaultGateway
    /// </summary>
    DefaultGateway = 11,
    /// <summary>
    /// ConfigurationItems - Antenna
    /// </summary>
    Antenna = 12,
    /// <summary>
    /// ConfigurationItems - BoardMacAddress
    /// </summary>
    BoardMacAddress = 13,
    /// <summary>
    /// ConfigurationItems - SoftApMacAddress
    /// </summary>
    SoftApMacAddress = 14
};

/// <summary>
/// WiFi reason codes
/// </summary>
public enum WiFiReasons
{
    /// <summary>
    /// WiFiReasons - Unspecified
    /// </summary>
    Unspecified = 1,
    /// <summary>
    /// WiFiReasons - AuthenticationExpired
    /// </summary>
    AuthenticationExpired = 2,
    /// <summary>
    /// WiFiReasons - AuthenticationLeave
    /// </summary>
    AuthenticationLeave = 3,
    /// <summary>
    /// WiFiReasons - AssociationExpired
    /// </summary>
    AssociationExpired = 4,
    /// <summary>
    /// WiFiReasons - AssociationTooMany
    /// </summary>
    AssociationTooMany = 5,
    /// <summary>
    /// WiFiReasons - NotAuthenticated
    /// </summary>
    NotAuthenticated = 6,
    /// <summary>
    /// WiFiReasons - NotAssociated
    /// </summary>
    NotAssociated = 7,
    /// <summary>
    /// WiFiReasons - AssociationLeave
    /// </summary>
    AssociationLeave = 8,
    /// <summary>
    /// WiFiReasons - AssociationNotAuthorized
    /// </summary>
    AssociationNotAuthorized = 9,
    /// <summary>
    /// WiFiReasons - DisassociatedPowerCapabilityBad
    /// </summary>
    DisassociatedPowerCapabilityBad = 10,
    /// <summary>
    /// WiFiReasons - DisassociatedSupplementaryChannelBad
    /// </summary>
    DisassociatedSupplementaryChannelBad = 11,
    /// <summary>
    /// WiFiReasons - InvalidElement
    /// </summary>
    InvalidElement = 13,
    /// <summary>
    /// WiFiReasons - MessageIntegrityCodeFailure
    /// </summary>
    MessageIntegrityCodeFailure = 14,
    /// <summary>
    /// WiFiReasons - FourWayHandshakeTimeout
    /// </summary>
    FourWayHandshakeTimeout = 15,
    /// <summary>
    /// WiFiReasons - GroupKeyUpdateTimeout
    /// </summary>
    GroupKeyUpdateTimeout = 16,
    /// <summary>
    /// WiFiReasons - InvalidElementInFourWayHandshake
    /// </summary>
    InvalidElementInFourWayHandshake = 17,
    /// <summary>
    /// WiFiReasons - InvalidGroupCipher
    /// </summary>
    InvalidGroupCipher = 18,
    /// <summary>
    /// WiFiReasons - InvalidPairwiseCipher
    /// </summary>
    InvalidPairwiseCipher = 19,
    /// <summary>
    /// WiFiReasons - InvalidAkmp
    /// </summary>
    InvalidAkmp = 20,
    /// <summary>
    /// WiFiReasons - UnsupportedRsneVersion
    /// </summary>
    UnsupportedRsneVersion = 21,
    /// <summary>
    /// WiFiReasons - InvalidRsneCapabilities
    /// </summary>
    InvalidRsneCapabilities = 22,
    /// <summary>
    /// WiFiReasons - Authentication801Failed
    /// </summary>
    Authentication801Failed = 23,
    /// <summary>
    /// WiFiReasons - CipherSuiteRejected
    /// </summary>
    CipherSuiteRejected = 24,
    /// <summary>
    /// WiFiReasons - InvalidPmkid
    /// </summary>
    InvalidPmkid = 53,
    /// <summary>
    /// WiFiReasons - BeaconTimeout
    /// </summary>
    BeaconTimeout = 200,
    /// <summary>
    /// WiFiReasons - NoAccessPointFound
    /// </summary>
    NoAccessPointFound = 201,
    /// <summary>
    /// WiFiReasons - AuthenticationFailed
    /// </summary>
    AuthenticationFailed = 202,
    /// <summary>
    /// WiFiReasons - AssociationFailed
    /// </summary>
    AssociationFailed = 203,
    /// <summary>
    /// WiFiReasons - HandshakeTimeout
    /// </summary>
    HandshakeTimeout = 204,
    /// <summary>
    /// WiFiReasons - ConnectionFailed
    /// </summary>
    ConnectionFailed = 205,
    /// <summary>
    /// WiFiReasons - TsfReset
    /// </summary>
    TsfReset = 206
};

/// <summary>
/// Access point authentication method.
/// </summary>
public enum WiFiAuthenticationMode
{
    /// <summary>
    /// WiFiAuthenticationMode - Open
    /// </summary>
    Open = 0,
    /// <summary>
    /// WiFiAuthenticationMode - Wep
    /// </summary>
    Wep = 1,
    /// <summary>
    /// WiFiAuthenticationMode - WpaPsk
    /// </summary>
    WpaPsk = 2,
    /// <summary>
    /// WiFiAuthenticationMode - Wpa2Psk
    /// </summary>
    Wpa2Psk = 3,
    /// <summary>
    /// WiFiAuthenticationMode - WpaWpa2Psk
    /// </summary>
    WpaWpa2Psk = 4,
    /// <summary>
    /// WiFiAuthenticationMode - Wpa2Enterprise
    /// </summary>
    Wpa2Enterprise = 5,
    /// <summary>
    /// WiFiAuthenticationMode - Wpa3Psk
    /// </summary>
    Wpa3Psk = 6,
    /// <summary>
    /// WiFiAuthenticationMode - Wpa2Wpa3Psk
    /// </summary>
    Wpa2Wpa3Psk = 7
};

/// <summary>
/// WiFi Country Policy.
/// </summary>
public enum WiFiCountryPolicy
{
    /// <summary>
    /// WiFiCountryPolicy - Automatic
    /// </summary>
    Automatic = 0,
    /// <summary>
    /// WiFiCountryPolicy - Manual
    /// </summary>
    Manual = 1
};

/// <summary>
/// Location of the secondary channel in respect to the primary channel.
/// </summary>
public enum WiFiSecondChannel
{
    /// <summary>
    /// WiFiSecondChannel - None
    /// </summary>
    None = 0,
    /// <summary>
    /// WiFiSecondChannel - Above
    /// </summary>
    Above = 1,
    /// <summary>
    /// WiFiSecondChannel - Below
    /// </summary>
    Below = 2
};

/// <summary>
/// WiFiScanType
/// </summary>
public enum WiFiScanType
{
    /// <summary>
    /// WiFiScanType - Active
    /// </summary>
    Active = 0,
    /// <summary>
    /// WiFiScanType - Passive
    /// </summary>
    Passive = 1
};

/// <summary>
/// Types of antennas that can be selected.
/// </summary>
public enum AntennaTypes
{
    /// <summary>
    /// AntennaTypes - OnBoard
    /// </summary>
    OnBoard = 0,
    /// <summary>
    /// AntennaTypes - External
    /// </summary>
    External = 1,
    /// <summary>
    /// AntennaTypes - Max
    /// </summary>
    Max = 1
};

/// <summary>
/// Encryption method used by the access point.
/// </summary>
public enum WiFiCipherType
{
    /// <summary>
    /// WiFiCipherType - None
    /// </summary>
    None = 0,
    /// <summary>
    /// WiFiCipherType - Wep40
    /// </summary>
    Wep40 = 1,
    /// <summary>
    /// WiFiCipherType - Wep104
    /// </summary>
    Wep104 = 2,
    /// <summary>
    /// WiFiCipherType - Tkip
    /// </summary>
    Tkip = 3,
    /// <summary>
    /// WiFiCipherType - Ccmp
    /// </summary>
    Ccmp = 4,
    /// <summary>
    /// WiFiCipherType - TkipCcmp
    /// </summary>
    TkipCcmp = 5,
    /// <summary>
    /// WiFiCipherType - Unknown
    /// </summary>
    Unknown = 6
};

/// <summary>
/// Address family types for the low level sockets..
/// </summary>
public enum AddressFamilyType
{
    /// <summary>
    /// AddressFamilyType - AfUnspec
    /// </summary>
    AfUnspec = 0,
    /// <summary>
    /// AddressFamilyType - AfInet
    /// </summary>
    AfInet = 2,
    /// <summary>
    /// AddressFamilyType - AfInet6
    /// </summary>
    AfInet6 = 10
};

/// <summary>
/// Protocol family types for the low level sockets..
/// </summary>
public enum ProtocolFamilyType
{
    /// <summary>
    /// ProtocolFamilyType - PfUnspec
    /// </summary>
    PfUnspec = 0,
    /// <summary>
    /// ProtocolFamilyType - PfInet
    /// </summary>
    PfInet = 2,
    /// <summary>
    /// ProtocolFamilyType - PfInet6
    /// </summary>
    PfInet6 = 10
};

/// <summary>
/// Protocol types for the low level sockets.
/// </summary>
public enum IpProtocolType
{
    /// <summary>
    /// IpProtocolType - IpProtoIP
    /// </summary>
    IpProtoIP = 0,
    /// <summary>
    /// IpProtocolType - IpProtoIcmp
    /// </summary>
    IpProtoIcmp = 1,
    /// <summary>
    /// IpProtocolType - IpProtoTcp
    /// </summary>
    IpProtoTcp = 6,
    /// <summary>
    /// IpProtocolType - IpProtoUdp
    /// </summary>
    IpProtoUdp = 17,
    /// <summary>
    /// IpProtocolType - IpProtoIpV6
    /// </summary>
    IpProtoIpV6 = 41,
    /// <summary>
    /// IpProtocolType - IpProtoIcmpV6
    /// </summary>
    IpProtoIcmpV6 = 58,
    /// <summary>
    /// IpProtocolType - IpProtoUdpLite
    /// </summary>
    IpProtoUdpLite = 136,
    /// <summary>
    /// IpProtocolType - IpProtoNew
    /// </summary>
    IpProtoNew = 255
};

/// <summary>
/// Socket protocol types for the low level sockets.
/// </summary>
public enum SocketProtocolType
{
    /// <summary>
    /// SocketProtocolType - SockStream
    /// </summary>
    SockStream = 1,
    /// <summary>
    /// SocketProtocolType - SockDgram
    /// </summary>
    SockDgram = 2,
    /// <summary>
    /// SocketProtocolType - SockRaw
    /// </summary>
    SockRaw = 3
};

/// <summary>
/// Socket level types for the low level sockets.
/// </summary>
public enum SocketLevelType
{
    /// <summary>
    /// SocketLevelType - SolSocket
    /// </summary>
    SolSocket = 0xfff
};

/// <summary>
/// Socket option types for the low level sockets.
/// </summary>
public enum SocketOptionType
{
    /// <summary>
    /// SocketOptionType - SoDebug
    /// </summary>
    SoDebug = 0x0001,
    /// <summary>
    /// SocketOptionType - SoAcceptConn
    /// </summary>
    SoAcceptConn = 0x0002,
    /// <summary>
    /// SocketOptionType - SoDontRoute
    /// </summary>
    SoDontRoute = 0x0010,
    /// <summary>
    /// SocketOptionType - UseLoopback
    /// </summary>
    UseLoopback = 0x0040,
    /// <summary>
    /// SocketOptionType - SoLinger
    /// </summary>
    SoLinger = 0x0080,
    /// <summary>
    /// SocketOptionType - SoDontLinger
    /// </summary>
    SoDontLinger = 0xff7f,
    /// <summary>
    /// SocketOptionType - SoOobInline
    /// </summary>
    SoOobInline = 0x0100,
    /// <summary>
    /// SocketOptionType - SoReusePort
    /// </summary>
    SoReusePort = 0x0200,
    /// <summary>
    /// SocketOptionType - SoSndBuf
    /// </summary>
    SoSndBuf = 0x1001,
    /// <summary>
    /// SocketOptionType - SoRcvBuf
    /// </summary>
    SoRcvBuf = 0x1002,
    /// <summary>
    /// SocketOptionType - SoSndLoWat
    /// </summary>
    SoSndLoWat = 0x1003,
    /// <summary>
    /// SocketOptionType - SoSRcvLoWat
    /// </summary>
    SoSRcvLoWat = 0x1004,
    /// <summary>
    /// SocketOptionType - SoSndTimeO
    /// </summary>
    SoSndTimeO = 0x1005,
    /// <summary>
    /// SocketOptionType - SoRcvTimeO
    /// </summary>
    SoRcvTimeO = 0x1006,
    /// <summary>
    /// SocketOptionType - SoError
    /// </summary>
    SoError = 0x1007,
    /// <summary>
    /// SocketOptionType - SoType
    /// </summary>
    SoType = 0x1008,
    /// <summary>
    /// SocketOptionType - SoConTimeO
    /// </summary>
    SoConTimeO = 0x1009,
    /// <summary>
    /// SocketOptionType - SoNoCheck
    /// </summary>
    SoNoCheck = 0x100a
};

/// <summary>
/// ESP32 Error codes (errno).
/// </summary>
public enum Esp32ErrorCodes
{
    /// <summary>
    /// Esp32ErrorCodes - Perm
    /// </summary>
    Perm = 1,
    /// <summary>
    /// Esp32ErrorCodes - NoEnt
    /// </summary>
    NoEnt = 2,
    /// <summary>
    /// Esp32ErrorCodes - Srch
    /// </summary>
    Srch = 3,
    /// <summary>
    /// Esp32ErrorCodes - Intr
    /// </summary>
    Intr = 4,
    /// <summary>
    /// Esp32ErrorCodes - IO
    /// </summary>
    IO = 5,
    /// <summary>
    /// Esp32ErrorCodes - NxIO
    /// </summary>
    NxIO = 6,
    /// <summary>
    /// Esp32ErrorCodes - TooBig
    /// </summary>
    TooBig = 7,
    /// <summary>
    /// Esp32ErrorCodes - NoExec
    /// </summary>
    NoExec = 8,
    /// <summary>
    /// Esp32ErrorCodes - BadF
    /// </summary>
    BadF = 9,
    /// <summary>
    /// Esp32ErrorCodes - Child
    /// </summary>
    Child = 10,
    /// <summary>
    /// Esp32ErrorCodes - Again
    /// </summary>
    Again = 11,
    /// <summary>
    /// Esp32ErrorCodes - NoMem
    /// </summary>
    NoMem = 12,
    /// <summary>
    /// Esp32ErrorCodes - Acces
    /// </summary>
    Acces = 13,
    /// <summary>
    /// Esp32ErrorCodes - Fault
    /// </summary>
    Fault = 14,
    /// <summary>
    /// Esp32ErrorCodes - Busy
    /// </summary>
    Busy = 16,
    /// <summary>
    /// Esp32ErrorCodes - Exist
    /// </summary>
    Exist = 17,
    /// <summary>
    /// Esp32ErrorCodes - XDev
    /// </summary>
    XDev = 18,
    /// <summary>
    /// Esp32ErrorCodes - NoDev
    /// </summary>
    NoDev = 19,
    /// <summary>
    /// Esp32ErrorCodes - NotDir
    /// </summary>
    NotDir = 20,
    /// <summary>
    /// Esp32ErrorCodes - IsDir
    /// </summary>
    IsDir = 21,
    /// <summary>
    /// Esp32ErrorCodes - InVal
    /// </summary>
    InVal = 22,
    /// <summary>
    /// Esp32ErrorCodes - NFile
    /// </summary>
    NFile = 23,
    /// <summary>
    /// Esp32ErrorCodes - MFile
    /// </summary>
    MFile = 24,
    /// <summary>
    /// Esp32ErrorCodes - NoTty
    /// </summary>
    NoTty = 25,
    /// <summary>
    /// Esp32ErrorCodes - TxtBsy
    /// </summary>
    TxtBsy = 26,
    /// <summary>
    /// Esp32ErrorCodes - FBig
    /// </summary>
    FBig = 27,
    /// <summary>
    /// Esp32ErrorCodes - NoSpc
    /// </summary>
    NoSpc = 28,
    /// <summary>
    /// Esp32ErrorCodes - SPipe
    /// </summary>
    SPipe = 29,
    /// <summary>
    /// Esp32ErrorCodes - RoFs
    /// </summary>
    RoFs = 30,
    /// <summary>
    /// Esp32ErrorCodes - MLink
    /// </summary>
    MLink = 31,
    /// <summary>
    /// Esp32ErrorCodes - Pipe
    /// </summary>
    Pipe = 32,
    /// <summary>
    /// Esp32ErrorCodes - Dom
    /// </summary>
    Dom = 33,
    /// <summary>
    /// Esp32ErrorCodes - Range
    /// </summary>
    Range = 34,
    /// <summary>
    /// Esp32ErrorCodes - NoMsg
    /// </summary>
    NoMsg = 35,
    /// <summary>
    /// Esp32ErrorCodes - IdRm
    /// </summary>
    IdRm = 36,
    /// <summary>
    /// Esp32ErrorCodes - DeadLck
    /// </summary>
    DeadLck = 45,
    /// <summary>
    /// Esp32ErrorCodes - NoLock
    /// </summary>
    NoLock = 46,
    /// <summary>
    /// Esp32ErrorCodes - NoStr
    /// </summary>
    NoStr = 60,
    /// <summary>
    /// Esp32ErrorCodes - NoData
    /// </summary>
    NoData = 61,
    /// <summary>
    /// Esp32ErrorCodes - Time
    /// </summary>
    Time = 62,
    /// <summary>
    /// Esp32ErrorCodes - NoSr
    /// </summary>
    NoSr = 63,
    /// <summary>
    /// Esp32ErrorCodes - NoLink
    /// </summary>
    NoLink = 67,
    /// <summary>
    /// Esp32ErrorCodes - Proto
    /// </summary>
    Proto = 71,
    /// <summary>
    /// Esp32ErrorCodes - MultiHop
    /// </summary>
    MultiHop = 74,
    /// <summary>
    /// Esp32ErrorCodes - BadMsg
    /// </summary>
    BadMsg = 77,
    /// <summary>
    /// Esp32ErrorCodes - FType
    /// </summary>
    FType = 79,
    /// <summary>
    /// Esp32ErrorCodes - NoSys
    /// </summary>
    NoSys = 88,
    /// <summary>
    /// Esp32ErrorCodes - NotEmpty
    /// </summary>
    NotEmpty = 90,
    /// <summary>
    /// Esp32ErrorCodes - NameTooLong
    /// </summary>
    NameTooLong = 91,
    /// <summary>
    /// Esp32ErrorCodes - Loop
    /// </summary>
    Loop = 92,
    /// <summary>
    /// Esp32ErrorCodes - OpNotSupport
    /// </summary>
    OpNotSupport = 95,
    /// <summary>
    /// Esp32ErrorCodes - PNoSupport
    /// </summary>
    PNoSupport = 96,
    /// <summary>
    /// Esp32ErrorCodes - ConnReset
    /// </summary>
    ConnReset = 104,
    /// <summary>
    /// Esp32ErrorCodes - NoBufs
    /// </summary>
    NoBufs = 105,
    /// <summary>
    /// Esp32ErrorCodes - AfNoSupport
    /// </summary>
    AfNoSupport = 106,
    /// <summary>
    /// Esp32ErrorCodes - ProtoType
    /// </summary>
    ProtoType = 107,
    /// <summary>
    /// Esp32ErrorCodes - NotSock
    /// </summary>
    NotSock = 108,
    /// <summary>
    /// Esp32ErrorCodes - NoProtoOpt
    /// </summary>
    NoProtoOpt = 109,
    /// <summary>
    /// Esp32ErrorCodes - ConnRefused
    /// </summary>
    ConnRefused = 111,
    /// <summary>
    /// Esp32ErrorCodes - AddrInUse
    /// </summary>
    AddrInUse = 112,
    /// <summary>
    /// Esp32ErrorCodes - ConnAborted
    /// </summary>
    ConnAborted = 113,
    /// <summary>
    /// Esp32ErrorCodes - NetUnreach
    /// </summary>
    NetUnreach = 114,
    /// <summary>
    /// Esp32ErrorCodes - NetDown
    /// </summary>
    NetDown = 115,
    /// <summary>
    /// Esp32ErrorCodes - TimedOut
    /// </summary>
    TimedOut = 116,
    /// <summary>
    /// Esp32ErrorCodes - HostDown
    /// </summary>
    HostDown = 117,
    /// <summary>
    /// Esp32ErrorCodes - HostUnreach
    /// </summary>
    HostUnreach = 118,
    /// <summary>
    /// Esp32ErrorCodes - InProgress
    /// </summary>
    InProgress = 119,
    /// <summary>
    /// Esp32ErrorCodes - Already
    /// </summary>
    Already = 120,
    /// <summary>
    /// Esp32ErrorCodes - DestAddrReq
    /// </summary>
    DestAddrReq = 121,
    /// <summary>
    /// Esp32ErrorCodes - MsgSize
    /// </summary>
    MsgSize = 122,
    /// <summary>
    /// Esp32ErrorCodes - ProtoNoSupport
    /// </summary>
    ProtoNoSupport = 123,
    /// <summary>
    /// Esp32ErrorCodes - AddrNotAvail
    /// </summary>
    AddrNotAvail = 125,
    /// <summary>
    /// Esp32ErrorCodes - NetReset
    /// </summary>
    NetReset = 126,
    /// <summary>
    /// Esp32ErrorCodes - NotConn
    /// </summary>
    NotConn = 128,
    /// <summary>
    /// Esp32ErrorCodes - TooManyRefs
    /// </summary>
    TooManyRefs = 129,
    /// <summary>
    /// Esp32ErrorCodes - DQuot
    /// </summary>
    DQuot = 132,
    /// <summary>
    /// Esp32ErrorCodes - Stale
    /// </summary>
    Stale = 133,
    /// <summary>
    /// Esp32ErrorCodes - NotSup
    /// </summary>
    NotSup = 134,
    /// <summary>
    /// Esp32ErrorCodes - IlSeq
    /// </summary>
    IlSeq = 138,
    /// <summary>
    /// Esp32ErrorCodes - Overflow
    /// </summary>
    Overflow = 139,
    /// <summary>
    /// Esp32ErrorCodes - Canceled
    /// </summary>
    Cancelled = 140,
    /// <summary>
    /// Esp32ErrorCodes - NotRecoverable
    /// </summary>
    NotRecoverable = 141,
    /// <summary>
    /// Esp32ErrorCodes - OwnerDead
    /// </summary>
    OwnerDead = 142,
    /// <summary>
    /// Esp32ErrorCodes - WouldBlock
    /// </summary>
    WouldBlock = 11
};

/// <summary>
/// ESP32 Error codes (errno).
/// </summary>
public enum Esp32ResetCodes
{
    /// <summary>
    /// Esp32ResetCodes - Unknown
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// Esp32ResetCodes - PowerOn
    /// </summary>
    PowerOn = 1,
    /// <summary>
    /// Esp32ResetCodes - ExternalGpio
    /// </summary>
    ExternalGpio = 2,
    /// <summary>
    /// Esp32ResetCodes - Software
    /// </summary>
    Software = 3,
    /// <summary>
    /// Esp32ResetCodes - Panic
    /// </summary>
    Panic = 4,
    /// <summary>
    /// Esp32ResetCodes - InterruptWatchdog
    /// </summary>
    InterruptWatchdog = 5,
    /// <summary>
    /// Esp32ResetCodes - TaskWatchdog
    /// </summary>
    TaskWatchdog = 6,
    /// <summary>
    /// Esp32ResetCodes - OtherWatchdog
    /// </summary>
    OtherWatchdog = 7,
    /// <summary>
    /// Esp32ResetCodes - DeepSleep
    /// </summary>
    DeepSleep = 8,
    /// <summary>
    /// Esp32ResetCodes - Brownout
    /// </summary>
    Brownout = 9,
    /// <summary>
    /// Esp32ResetCodes - SDIO
    /// </summary>
    SDIO = 10
};
