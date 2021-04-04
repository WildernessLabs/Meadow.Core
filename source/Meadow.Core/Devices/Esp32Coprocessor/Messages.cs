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
    public struct SystemConfiguration
    {
        public String SoftwareVersion;
        public Byte MaximumMessageQueueLength;
        public Byte AutomaticallyStartNetwork;
        public Byte AutomaticallyReconnect;
        public UInt32 MaximumRetryCount;
        public Byte Antenna;
        public Byte[] BoardMacAddress;
        public Byte[] SoftApMacAddress;
        public String DeviceName;
        public String DefaultAccessPoint;
        public String NtpServer;
        public Int32 GetTimeAtStartup;
        public Byte UseDhcp;
        public UInt32 StaticIpAddress;
        public UInt32 DnsServer;
        public UInt32 DefaultGateway;
    };

    public struct ConfigurationValue
    {
        public UInt32 Item;
        public UInt32 ValueLength;
        public byte[] Value;
    };

    public struct WiFiCredentials
    {
        public String NetworkName;
        public String Password;
    };

    public struct ConnectDisconnectData
    {
        public UInt32 IpAddress;
        public UInt32 SubnetMask;
        public UInt32 Gateway;
        public string Ssid;
        public Byte[] Bssid;
        public Byte Channel;
        public Byte AuthenticationMode;
        public Byte Connect;
        public UInt32 Reason;
    };

    public struct AccessPoint
    {
        public string Ssid;
        public Byte[] Bssid;
        public Byte PrimaryChannel;
        public Byte SecondaryChannel;
        public sbyte Rssi;
        public Byte AuthenticationMode;
        public UInt32 Protocols;
    };

    public struct AccessPointList
    {
        public UInt32 NumberOfAccessPoints;
        public UInt32 AccessPointsLength;
        public byte[] AccessPoints;
    };

    public struct SockAddr
    {
        public Byte Family;
        public UInt16 Port;
        public UInt32 Ip4Address;
        public UInt32 FlowInfo;
        public Byte[] Ip6Address;
        public UInt32 ScopeID;
    };

    public struct AddrInfo
    {
        public UInt32 MyHeapAddress;
        public Int32 Flags;
        public Int32 Family;
        public Int32 SocketType;
        public Int32 Protocol;
        public UInt32 AddrLen;
        public UInt32 AddrLength;
        public byte[] Addr;
        public String CanonName;
        public UInt32 Next;
    };

    public struct GetAddrInfoRequest
    {
        public String NodeName;
        public String ServName;
        public UInt32 HintsLength;
        public byte[] Hints;
        public UInt32 ResultLength;
        public byte[] Result;
    };

    public struct GetAddrInfoResponse
    {
        public Int32 AddrInfoResponseErrno;
        public UInt32 ResLength;
        public byte[] Res;
    };

    public struct SocketRequest
    {
        public UInt32 AddressInformation;
        public Int32 Domain;
        public Int32 Type;
        public Int32 Protocol;
    };

    public struct IntegerResponse
    {
        public Int32 Result;
    };

    public struct IntegerAndErrnoResponse
    {
        public Int32 Result;
        public Int32 ResponseErrno;
    };

    public struct ConnectRequest
    {
        public Int32 SocketHandle;
        public UInt32 AddrLength;
        public byte[] Addr;
    };

    public struct FreeAddrInfoRequest
    {
        public UInt32 AddrInfoAddress;
    };

    public struct TimeVal
    {
        public UInt32 TvSec;
        public UInt32 TvUsec;
    };

    public struct SetSockOptRequest
    {
        public Int32 SocketHandle;
        public Int32 Level;
        public Int32 OptionName;
        public UInt32 OptionValueLength;
        public byte[] OptionValue;
        public Int32 OptionLen;
    };

    public struct Linger
    {
        public Int32 LOnOff;
        public Int32 LLinger;
    };

    public struct WriteRequest
    {
        public Int32 SocketHandle;
        public UInt32 BufferLength;
        public byte[] Buffer;
        public Int32 Count;
    };

    public struct ReadRequest
    {
        public Int32 SocketHandle;
        public Int32 Count;
    };

    public struct ReadResponse
    {
        public UInt32 BufferLength;
        public byte[] Buffer;
        public Int32 ReadResponseResult;
        public Int32 ReadResponseErrno;
    };

    public struct CloseRequest
    {
        public Int32 SocketHandle;
    };

    public struct GetBatteryChargeLevelResponse
    {
        public UInt32 Level;
    };

    public struct SendRequest
    {
        public Int32 SocketHandle;
        public UInt32 BufferLength;
        public byte[] Buffer;
        public Int32 Length;
        public Int32 Flags;
    };

    public struct SendToRequest
    {
        public Int32 SocketHandle;
        public UInt32 BufferLength;
        public byte[] Buffer;
        public Int32 Length;
        public Int32 Flags;
        public UInt32 DestinationAddressLength;
        public byte[] DestinationAddress;
    };

    public struct RecvFromRequest
    {
        public Int32 SocketHandle;
        public Int32 Length;
        public Int32 Flags;
        public Int32 GetSourceAddress;
    };

    public struct RecvFromResponse
    {
        public UInt32 BufferLength;
        public byte[] Buffer;
        public Int32 Result;
        public Int32 ResponseErrno;
        public UInt32 SourceAddressLength;
        public byte[] SourceAddress;
        public UInt32 SourceAddressLen;
    };

    public struct PollRequest
    {
        public Int32 SocketHandle;
        public UInt16 Events;
        public Int32 Timeout;
        public Int32 Setup;
        public UInt32 SetupMessageId;
    };

    public struct PollResponse
    {
        public UInt16 ReturnedEvents;
        public Int32 Result;
        public Int32 ResponseErrno;
    };

    public struct InterruptPollResponse
    {
        public Int32 SocketHandle;
        public Int32 Result;
        public Int32 ResponseErrno;
        public UInt16 ReturnedEvents;
        public UInt32 SetupMessageId;
    };

    public struct ListenRequest
    {
        public Int32 SocketHandle;
        public Int32 BackLog;
    };

    public struct BindRequest
    {
        public Int32 SocketHandle;
        public UInt32 AddrLength;
        public byte[] Addr;
    };

    public struct AcceptRequest
    {
        public Int32 SocketHandle;
    };

    public struct AcceptResponse
    {
        public UInt32 AddrLength;
        public byte[] Addr;
        public Int32 Result;
        public Int32 ResponseErrno;
    };

    public struct IoctlRequest
    {
        public Int32 Command;
    };

    public struct IoctlResponse
    {
        public UInt32 AddrLength;
        public byte[] Addr;
        public Int32 Flags;
    };

    public struct GetSockNameRequest
    {
        public Int32 SocketHandle;
    };

    public struct GetSockNameResponse
    {
        public UInt32 AddrLength;
        public byte[] Addr;
        public Int32 Result;
        public Int32 ResponseErrno;
    };

    public struct EventData
    {
        public Byte Interface;
        public UInt32 Function;
        public UInt32 StatusCode;
        public UInt32 Payload;
        public UInt32 PayloadLength;
    };

    public struct SetAntennaRequest
    {
        public Byte Antenna;
        public Byte Persist;
    };

    public struct BTStackConfig
    {
        public String Config;
    };

    public struct BTDataWriteRequest
    {
        public UInt16 Handle;
        public UInt32 DataLength;
        public byte[] Data;
    };

    public struct BTGetHandlesResponse
    {
        public UInt16 HandleCount;
        public UInt32 HandlesLength;
        public byte[] Handles;
    };

    public struct BTServerDataSet
    {
        public UInt16 Handle;
        public UInt32 DataLength;
        public byte[] Data;
    };

}
