using System;

namespace Meadow.Hardware
{
    public interface IDeviceChannelManager
    {
        Tuple<bool, string> ReservePin(IPin pin, ChannelConfigurationType configType);
        bool ReleasePin(IPin pin);
        Tuple<bool, string> ReservePwm(IPin pin, IPwmChannelInfo channelInfo, float frequency);
        void BeforeStartPwm(IPwmChannelInfo info);
        void AfterStartPwm(IPwmChannelInfo info, IMeadowIOController ioController);
    }
}
