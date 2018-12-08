using System;
using System.Collections;

namespace Meadow.Hardware.Communications
{
    public class OneWire
    {
        public OneWire(DigitalOutputPort port) { throw new NotImplementedException(); }

        public int AcquireEx() { throw new NotImplementedException(); }
        public ArrayList FindAllDevices() { throw new NotImplementedException(); }
        public int FindFirstDevice(bool performResetBeforeSearch, bool searchWithAlarmCommand) { throw new NotImplementedException(); }
        public int FindNextDevice(bool performResetBeforeSearch, bool searchWithAlarmCommand) { throw new NotImplementedException(); }
        public int ReadByte() { throw new NotImplementedException(); }
        public int Release() { throw new NotImplementedException(); }
        public int SerialNum(byte[] SNum, bool read) { throw new NotImplementedException(); }
        public int TouchBit(int sendbit) { throw new NotImplementedException(); }
        public int TouchByte(int sendbyte) { throw new NotImplementedException(); }
        public int TouchReset() { throw new NotImplementedException(); }
        public int WriteByte(int sendbyte) { throw new NotImplementedException(); }
    }
}
