//
//      
//
//      Message encoders and decoders.
//
//      *************************** WARNING ***************************
//
//      THIS FILE IS AUTOMATICALLY GENERATED.  DO NOT EDIT THIS FILE AS
//      CHANGES WILL BE OVERWRITTEN.
//
//      *************************** WARNING ***************************
//
using System;

namespace Meadow.Devices.Esp32.MessagePayloads
{
    /// <summary>
    /// Structure to hold SystemConfiguration data.
    /// </summary>
    public struct SystemConfiguration
    {
        /// <summary>
        /// MaximumMessageQueueLength element.
        /// </summary>
        public Byte MaximumMessageQueueLength;
        /// <summary>
        /// MaximumRetryCount element.
        /// </summary>
        public Int32 MaximumRetryCount;
        /// <summary>
        /// Antenna element.
        /// </summary>
        public Byte Antenna;
        /// <summary>
        /// BoardMacAddress element.
        /// </summary>
        public Byte[] BoardMacAddress;
        /// <summary>
        /// SoftApMacAddress element.
        /// </summary>
        public Byte[] SoftApMacAddress;
        /// <summary>
        /// BluetoothMacAddress element.
        /// </summary>
        public Byte[] BluetoothMacAddress;
        /// <summary>
        /// DeviceName element.
        /// </summary>
        public String DeviceName;
        /// <summary>
        /// DefaultAccessPoint element.
        /// </summary>
        public String DefaultAccessPoint;
        /// <summary>
        /// ResetReason element.
        /// </summary>
        public Byte ResetReason;
        /// <summary>
        /// VersionMajor element.
        /// </summary>
        public UInt32 VersionMajor;
        /// <summary>
        /// VersionMinor element.
        /// </summary>
        public UInt32 VersionMinor;
        /// <summary>
        /// VersionRevision element.
        /// </summary>
        public UInt32 VersionRevision;
        /// <summary>
        /// VersionBuild element.
        /// </summary>
        public UInt32 VersionBuild;
        /// <summary>
        /// BuildDay element.
        /// </summary>
        public Byte BuildDay;
        /// <summary>
        /// BuildMonth element.
        /// </summary>
        public Byte BuildMonth;
        /// <summary>
        /// BuildYear element.
        /// </summary>
        public Byte BuildYear;
        /// <summary>
        /// BuildHour element.
        /// </summary>
        public Byte BuildHour;
        /// <summary>
        /// BuildMinute element.
        /// </summary>
        public Byte BuildMinute;
        /// <summary>
        /// BuildSecond element.
        /// </summary>
        public Byte BuildSecond;
        /// <summary>
        /// BuildHash element.
        /// </summary>
        public UInt32 BuildHash;
        /// <summary>
        /// BuildBranchName element.
        /// </summary>
        public String BuildBranchName;
    };

    /// <summary>
    /// Structure to hold ConfigurationValue data.
    /// </summary>
    public struct ConfigurationValue
    {
        /// <summary>
        /// Item element.
        /// </summary>
        public UInt32 Item;
        /// <summary>
        /// Value element length.
        /// </summary>
        public UInt32 ValueLength;
        /// <summary>
        /// Value element.
        /// </summary>
        public byte[] Value;
    };

    /// <summary>
    /// Structure to hold ErrorEvent data.
    /// </summary>
    public struct ErrorEvent
    {
        /// <summary>
        /// ErrorCode element.
        /// </summary>
        public UInt32 ErrorCode;
        /// <summary>
        /// Interface element.
        /// </summary>
        public Byte Interface;
        /// <summary>
        /// ErrorData element length.
        /// </summary>
        public UInt32 ErrorDataLength;
        /// <summary>
        /// ErrorData element.
        /// </summary>
        public byte[] ErrorData;
    };

    /// <summary>
    /// Structure to hold AccessPointInformation data.
    /// </summary>
    public struct AccessPointInformation
    {
        /// <summary>
        /// NetworkName element.
        /// </summary>
        public String NetworkName;
        /// <summary>
        /// Password element.
        /// </summary>
        public String Password;
        /// <summary>
        /// IpAddress element.
        /// </summary>
        public UInt32 IpAddress;
        /// <summary>
        /// SubnetMask element.
        /// </summary>
        public UInt32 SubnetMask;
        /// <summary>
        /// Gateway element.
        /// </summary>
        public UInt32 Gateway;
        /// <summary>
        /// WiFiAuthenticationMode element.
        /// </summary>
        public Byte WiFiAuthenticationMode;
        /// <summary>
        /// Channel element.
        /// </summary>
        public Byte Channel;
        /// <summary>
        /// Hidden element.
        /// </summary>
        public Byte Hidden;
    };

    /// <summary>
    /// Structure to hold DisconnectFromAccessPointRequest data.
    /// </summary>
    public struct DisconnectFromAccessPointRequest
    {
        /// <summary>
        /// TurnOffWiFiInterface element.
        /// </summary>
        public Byte TurnOffWiFiInterface;
    };

    /// <summary>
    /// Structure to hold ConnectEventData data.
    /// </summary>
    public struct ConnectEventData
    {
        /// <summary>
        /// IpAddress element.
        /// </summary>
        public UInt32 IpAddress;
        /// <summary>
        /// SubnetMask element.
        /// </summary>
        public UInt32 SubnetMask;
        /// <summary>
        /// Gateway element.
        /// </summary>
        public UInt32 Gateway;
        /// <summary>
        /// Ssid element.
        /// </summary>
        public string Ssid;
        /// <summary>
        /// Bssid element.
        /// </summary>
        public Byte[] Bssid;
        /// <summary>
        /// Channel element.
        /// </summary>
        public Byte Channel;
        /// <summary>
        /// AuthenticationMode element.
        /// </summary>
        public Byte AuthenticationMode;
        /// <summary>
        /// Reason element.
        /// </summary>
        public UInt32 Reason;
    };


    /// <summary>
    /// Structure to hold NodeConnectionChangeEventData data.
    /// </summary>
    public struct NodeConnectionChangeEventData
    {
        /// <summary>
        /// IpAddress element.
        /// </summary>
        public UInt32 IpAddress;
        /// <summary>
        /// MacAddress element.
        /// </summary>
        public Byte[] MacAddress;
    };

    /// <summary>
    /// Structure to hold DisconnectEventData data.
    /// </summary>
    public struct DisconnectEventData
    {
        /// <summary>
        /// Ssid element.
        /// </summary>
        public string Ssid;
        /// <summary>
        /// SsidLength element.
        /// </summary>
        public Byte SsidLength;
        /// <summary>
        /// Bssid element.
        /// </summary>
        public Byte[] Bssid;
        /// <summary>
        /// Rssi element.
        /// </summary>
        public Byte Rssi;
        /// <summary>
        /// Reason element.
        /// </summary>
        public Byte Reason;
    };

    /// <summary>
    /// Structure to hold AccessPoint data.
    /// </summary>
    public struct AccessPoint
    {
        /// <summary>
        /// Ssid element.
        /// </summary>
        public string Ssid;
        /// <summary>
        /// Bssid element.
        /// </summary>
        public Byte[] Bssid;
        /// <summary>
        /// PrimaryChannel element.
        /// </summary>
        public Byte PrimaryChannel;
        /// <summary>
        /// SecondaryChannel element.
        /// </summary>
        public Byte SecondaryChannel;
        /// <summary>
        /// Rssi element.
        /// </summary>
        public sbyte Rssi;
        /// <summary>
        /// AuthenticationMode element.
        /// </summary>
        public Byte AuthenticationMode;
        /// <summary>
        /// Protocols element.
        /// </summary>
        public UInt32 Protocols;
    };

    /// <summary>
    /// Structure to hold AccessPointList data.
    /// </summary>
    public struct AccessPointList
    {
        /// <summary>
        /// NumberOfAccessPoints element.
        /// </summary>
        public UInt32 NumberOfAccessPoints;
        /// <summary>
        /// AccessPoints element length.
        /// </summary>
        public UInt32 AccessPointsLength;
        /// <summary>
        /// AccessPoints element.
        /// </summary>
        public byte[] AccessPoints;
    };

    /// <summary>
    /// Structure to hold SockAddr data.
    /// </summary>
    public struct SockAddr
    {
        /// <summary>
        /// Family element.
        /// </summary>
        public Byte Family;
        /// <summary>
        /// Port element.
        /// </summary>
        public UInt16 Port;
        /// <summary>
        /// Ip4Address element.
        /// </summary>
        public UInt32 Ip4Address;
        /// <summary>
        /// FlowInfo element.
        /// </summary>
        public UInt32 FlowInfo;
        /// <summary>
        /// Ip6Address element.
        /// </summary>
        public Byte[] Ip6Address;
        /// <summary>
        /// ScopeID element.
        /// </summary>
        public UInt32 ScopeID;
    };

    /// <summary>
    /// Structure to hold AddrInfo data.
    /// </summary>
    public struct AddrInfo
    {
        /// <summary>
        /// MyHeapAddress element.
        /// </summary>
        public UInt32 MyHeapAddress;
        /// <summary>
        /// Flags element.
        /// </summary>
        public Int32 Flags;
        /// <summary>
        /// Family element.
        /// </summary>
        public Int32 Family;
        /// <summary>
        /// SocketType element.
        /// </summary>
        public Int32 SocketType;
        /// <summary>
        /// Protocol element.
        /// </summary>
        public Int32 Protocol;
        /// <summary>
        /// AddrLen element.
        /// </summary>
        public UInt32 AddrLen;
        /// <summary>
        /// Addr element length.
        /// </summary>
        public UInt32 AddrLength;
        /// <summary>
        /// Addr element.
        /// </summary>
        public byte[] Addr;
        /// <summary>
        /// CanonName element.
        /// </summary>
        public String CanonName;
        /// <summary>
        /// Next element.
        /// </summary>
        public UInt32 Next;
    };

    /// <summary>
    /// Structure to hold GetAddrInfoRequest data.
    /// </summary>
    public struct GetAddrInfoRequest
    {
        /// <summary>
        /// NodeName element.
        /// </summary>
        public String NodeName;
        /// <summary>
        /// ServName element.
        /// </summary>
        public String ServName;
        /// <summary>
        /// Hints element length.
        /// </summary>
        public UInt32 HintsLength;
        /// <summary>
        /// Hints element.
        /// </summary>
        public byte[] Hints;
    };

    /// <summary>
    /// Structure to hold GetAddrInfoResponse data.
    /// </summary>
    public struct GetAddrInfoResponse
    {
        /// <summary>
        /// AddrInfoResponseErrno element.
        /// </summary>
        public Int32 AddrInfoResponseErrno;
        /// <summary>
        /// Res element length.
        /// </summary>
        public UInt32 ResLength;
        /// <summary>
        /// Res element.
        /// </summary>
        public byte[] Res;
    };

    /// <summary>
    /// Structure to hold SocketRequest data.
    /// </summary>
    public struct SocketRequest
    {
        /// <summary>
        /// AddressInformation element.
        /// </summary>
        public UInt32 AddressInformation;
        /// <summary>
        /// Domain element.
        /// </summary>
        public Int32 Domain;
        /// <summary>
        /// Type element.
        /// </summary>
        public Int32 Type;
        /// <summary>
        /// Protocol element.
        /// </summary>
        public Int32 Protocol;
    };

    /// <summary>
    /// Structure to hold IntegerResponse data.
    /// </summary>
    public struct IntegerResponse
    {
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
    };

    /// <summary>
    /// Structure to hold IntegerAndErrnoResponse data.
    /// </summary>
    public struct IntegerAndErrnoResponse
    {
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
        /// <summary>
        /// ResponseErrno element.
        /// </summary>
        public Int32 ResponseErrno;
    };

    /// <summary>
    /// Structure to hold ConnectRequest data.
    /// </summary>
    public struct ConnectRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Addr element length.
        /// </summary>
        public UInt32 AddrLength;
        /// <summary>
        /// Addr element.
        /// </summary>
        public byte[] Addr;
    };

    /// <summary>
    /// Structure to hold FreeAddrInfoRequest data.
    /// </summary>
    public struct FreeAddrInfoRequest
    {
        /// <summary>
        /// AddrInfoAddress element.
        /// </summary>
        public UInt32 AddrInfoAddress;
    };

    /// <summary>
    /// Structure to hold TimeVal data.
    /// </summary>
    public struct TimeVal
    {
        /// <summary>
        /// TvSec element.
        /// </summary>
        public UInt32 TvSec;
        /// <summary>
        /// TvUsec element.
        /// </summary>
        public UInt32 TvUsec;
    };

    /// <summary>
    /// Structure to hold SetSockOptRequest data.
    /// </summary>
    public struct SetSockOptRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Level element.
        /// </summary>
        public Int32 Level;
        /// <summary>
        /// OptionName element.
        /// </summary>
        public Int32 OptionName;
        /// <summary>
        /// OptionValue element length.
        /// </summary>
        public UInt32 OptionValueLength;
        /// <summary>
        /// OptionValue element.
        /// </summary>
        public byte[] OptionValue;
        /// <summary>
        /// OptionLen element.
        /// </summary>
        public Int32 OptionLen;
    };

    /// <summary>
    /// Structure to hold GetSockOptRequest data.
    /// </summary>
    public struct GetSockOptRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Level element.
        /// </summary>
        public Int32 Level;
        /// <summary>
        /// OptionName element.
        /// </summary>
        public Int32 OptionName;
    };

    /// <summary>
    /// Structure to hold GetSockOptResponse data.
    /// </summary>
    public struct GetSockOptResponse
    {
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
        /// <summary>
        /// ResponseErrno element.
        /// </summary>
        public Int32 ResponseErrno;
        /// <summary>
        /// OptionValue element length.
        /// </summary>
        public UInt32 OptionValueLength;
        /// <summary>
        /// OptionValue element.
        /// </summary>
        public byte[] OptionValue;
        /// <summary>
        /// OptionLen element.
        /// </summary>
        public Int32 OptionLen;
    };

    /// <summary>
    /// Structure to hold Linger data.
    /// </summary>
    public struct Linger
    {
        /// <summary>
        /// LOnOff element.
        /// </summary>
        public Int32 LOnOff;
        /// <summary>
        /// LLinger element.
        /// </summary>
        public Int32 LLinger;
    };

    /// <summary>
    /// Structure to hold WriteRequest data.
    /// </summary>
    public struct WriteRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Buffer element length.
        /// </summary>
        public UInt32 BufferLength;
        /// <summary>
        /// Buffer element.
        /// </summary>
        public byte[] Buffer;
        /// <summary>
        /// Count element.
        /// </summary>
        public Int32 Count;
    };

    /// <summary>
    /// Structure to hold ReadRequest data.
    /// </summary>
    public struct ReadRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Count element.
        /// </summary>
        public Int32 Count;
    };

    /// <summary>
    /// Structure to hold ReadResponse data.
    /// </summary>
    public struct ReadResponse
    {
        /// <summary>
        /// Buffer element length.
        /// </summary>
        public UInt32 BufferLength;
        /// <summary>
        /// Buffer element.
        /// </summary>
        public byte[] Buffer;
        /// <summary>
        /// ReadResponseResult element.
        /// </summary>
        public Int32 ReadResponseResult;
        /// <summary>
        /// ReadResponseErrno element.
        /// </summary>
        public Int32 ReadResponseErrno;
    };

    /// <summary>
    /// Structure to hold CloseRequest data.
    /// </summary>
    public struct CloseRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
    };

    /// <summary>
    /// Structure to hold GetBatteryChargeLevelResponse data.
    /// </summary>
    public struct GetBatteryChargeLevelResponse
    {
        /// <summary>
        /// Level element.
        /// </summary>
        public UInt32 Level;
    };

    /// <summary>
    /// Structure to hold SendRequest data.
    /// </summary>
    public struct SendRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Buffer element length.
        /// </summary>
        public UInt32 BufferLength;
        /// <summary>
        /// Buffer element.
        /// </summary>
        public byte[] Buffer;
        /// <summary>
        /// Length element.
        /// </summary>
        public Int32 Length;
        /// <summary>
        /// Flags element.
        /// </summary>
        public Int32 Flags;
    };

    /// <summary>
    /// Structure to hold SendToRequest data.
    /// </summary>
    public struct SendToRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Buffer element length.
        /// </summary>
        public UInt32 BufferLength;
        /// <summary>
        /// Buffer element.
        /// </summary>
        public byte[] Buffer;
        /// <summary>
        /// Length element.
        /// </summary>
        public Int32 Length;
        /// <summary>
        /// Flags element.
        /// </summary>
        public Int32 Flags;
        /// <summary>
        /// DestinationAddress element length.
        /// </summary>
        public UInt32 DestinationAddressLength;
        /// <summary>
        /// DestinationAddress element.
        /// </summary>
        public byte[] DestinationAddress;
    };

    /// <summary>
    /// Structure to hold RecvFromRequest data.
    /// </summary>
    public struct RecvFromRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Length element.
        /// </summary>
        public Int32 Length;
        /// <summary>
        /// Flags element.
        /// </summary>
        public Int32 Flags;
        /// <summary>
        /// GetSourceAddress element.
        /// </summary>
        public Int32 GetSourceAddress;
    };

    /// <summary>
    /// Structure to hold RecvFromResponse data.
    /// </summary>
    public struct RecvFromResponse
    {
        /// <summary>
        /// Buffer element length.
        /// </summary>
        public UInt32 BufferLength;
        /// <summary>
        /// Buffer element.
        /// </summary>
        public byte[] Buffer;
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
        /// <summary>
        /// ResponseErrno element.
        /// </summary>
        public Int32 ResponseErrno;
        /// <summary>
        /// SourceAddress element length.
        /// </summary>
        public UInt32 SourceAddressLength;
        /// <summary>
        /// SourceAddress element.
        /// </summary>
        public byte[] SourceAddress;
        /// <summary>
        /// SourceAddressLen element.
        /// </summary>
        public UInt32 SourceAddressLen;
    };

    /// <summary>
    /// Structure to hold PollRequest data.
    /// </summary>
    public struct PollRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Events element.
        /// </summary>
        public UInt16 Events;
        /// <summary>
        /// Timeout element.
        /// </summary>
        public Int32 Timeout;
        /// <summary>
        /// Setup element.
        /// </summary>
        public Int32 Setup;
        /// <summary>
        /// SetupMessageId element.
        /// </summary>
        public UInt32 SetupMessageId;
    };

    /// <summary>
    /// Structure to hold PollResponse data.
    /// </summary>
    public struct PollResponse
    {
        /// <summary>
        /// ReturnedEvents element.
        /// </summary>
        public UInt16 ReturnedEvents;
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
        /// <summary>
        /// ResponseErrno element.
        /// </summary>
        public Int32 ResponseErrno;
    };

    /// <summary>
    /// Structure to hold InterruptPollResponse data.
    /// </summary>
    public struct InterruptPollResponse
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
        /// <summary>
        /// ResponseErrno element.
        /// </summary>
        public Int32 ResponseErrno;
        /// <summary>
        /// ReturnedEvents element.
        /// </summary>
        public UInt16 ReturnedEvents;
        /// <summary>
        /// SetupMessageId element.
        /// </summary>
        public UInt32 SetupMessageId;
    };

    /// <summary>
    /// Structure to hold ListenRequest data.
    /// </summary>
    public struct ListenRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// BackLog element.
        /// </summary>
        public Int32 BackLog;
    };

    /// <summary>
    /// Structure to hold BindRequest data.
    /// </summary>
    public struct BindRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
        /// <summary>
        /// Addr element length.
        /// </summary>
        public UInt32 AddrLength;
        /// <summary>
        /// Addr element.
        /// </summary>
        public byte[] Addr;
    };

    /// <summary>
    /// Structure to hold AcceptRequest data.
    /// </summary>
    public struct AcceptRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
    };

    /// <summary>
    /// Structure to hold AcceptResponse data.
    /// </summary>
    public struct AcceptResponse
    {
        /// <summary>
        /// Addr element length.
        /// </summary>
        public UInt32 AddrLength;
        /// <summary>
        /// Addr element.
        /// </summary>
        public byte[] Addr;
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
        /// <summary>
        /// ResponseErrno element.
        /// </summary>
        public Int32 ResponseErrno;
    };

    /// <summary>
    /// Structure to hold IoctlRequest data.
    /// </summary>
    public struct IoctlRequest
    {
        /// <summary>
        /// Command element.
        /// </summary>
        public Int32 Command;
    };

    /// <summary>
    /// Structure to hold IoctlResponse data.
    /// </summary>
    public struct IoctlResponse
    {
        /// <summary>
        /// Addr element length.
        /// </summary>
        public UInt32 AddrLength;
        /// <summary>
        /// Addr element.
        /// </summary>
        public byte[] Addr;
        /// <summary>
        /// Flags element.
        /// </summary>
        public Int32 Flags;
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
        /// <summary>
        /// ResponseErrno element.
        /// </summary>
        public Int32 ResponseErrno;
    };

    /// <summary>
    /// Structure to hold GetSockPeerNameRequest data.
    /// </summary>
    public struct GetSockPeerNameRequest
    {
        /// <summary>
        /// SocketHandle element.
        /// </summary>
        public Int32 SocketHandle;
    };

    /// <summary>
    /// Structure to hold GetSockPeerNameResponse data.
    /// </summary>
    public struct GetSockPeerNameResponse
    {
        /// <summary>
        /// Addr element length.
        /// </summary>
        public UInt32 AddrLength;
        /// <summary>
        /// Addr element.
        /// </summary>
        public byte[] Addr;
        /// <summary>
        /// Result element.
        /// </summary>
        public Int32 Result;
        /// <summary>
        /// ResponseErrno element.
        /// </summary>
        public Int32 ResponseErrno;
    };

    /// <summary>
    /// Structure to hold EventData data.
    /// </summary>
    public struct EventData
    {
        /// <summary>
        /// Interface element.
        /// </summary>
        public Byte Interface;
        /// <summary>
        /// Function element.
        /// </summary>
        public UInt32 Function;
        /// <summary>
        /// StatusCode element.
        /// </summary>
        public UInt32 StatusCode;
        /// <summary>
        /// MessageId element.
        /// </summary>
        public UInt32 MessageId;
    };

    /// <summary>
    /// Structure to hold EventDataPayload data.
    /// </summary>
    public struct EventDataPayload
    {
        /// <summary>
        /// MessageId element.
        /// </summary>
        public UInt32 MessageId;
        /// <summary>
        /// Payload element length.
        /// </summary>
        public UInt32 PayloadLength;
        /// <summary>
        /// Payload element.
        /// </summary>
        public byte[] Payload;
    };

    /// <summary>
    /// Structure to hold SetAntennaRequest data.
    /// </summary>
    public struct SetAntennaRequest
    {
        /// <summary>
        /// Antenna element.
        /// </summary>
        public Byte Antenna;
        /// <summary>
        /// Persist element.
        /// </summary>
        public Byte Persist;
    };

    /// <summary>
    /// Structure to hold BTStackConfig data.
    /// </summary>
    public struct BTStackConfig
    {
        /// <summary>
        /// Config element.
        /// </summary>
        public String Config;
    };

    /// <summary>
    /// Structure to hold BTDataWriteRequest data.
    /// </summary>
    public struct BTDataWriteRequest
    {
        /// <summary>
        /// Handle element.
        /// </summary>
        public UInt16 Handle;
        /// <summary>
        /// Data element length.
        /// </summary>
        public UInt32 DataLength;
        /// <summary>
        /// Data element.
        /// </summary>
        public byte[] Data;
    };

    /// <summary>
    /// Structure to hold BTGetHandlesResponse data.
    /// </summary>
    public struct BTGetHandlesResponse
    {
        /// <summary>
        /// HandleCount element.
        /// </summary>
        public UInt16 HandleCount;
        /// <summary>
        /// Handles element length.
        /// </summary>
        public UInt32 HandlesLength;
        /// <summary>
        /// Handles element.
        /// </summary>
        public byte[] Handles;
    };

    /// <summary>
    /// Structure to hold BTServerDataSet data.
    /// </summary>
    public struct BTServerDataSet
    {
        /// <summary>
        /// Handle element.
        /// </summary>
        public UInt16 Handle;
        /// <summary>
        /// SetData element length.
        /// </summary>
        public UInt32 SetDataLength;
        /// <summary>
        /// SetData element.
        /// </summary>
        public byte[] SetData;
    };

}
