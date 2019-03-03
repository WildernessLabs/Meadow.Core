//using System;
//using System.Threading;
////
////Work in progress
////
//namespace Meadow.Hardware.Communications
//{
//    public class SoftwareI2C : ICommunicationBus
//    {
//        BiDirectionalPort sdaPort;
//        BiDirectionalPort sclPort;

//        static int DELAY = 4;

//        public SoftwareI2C(IPin pinSda, IPin pinScl)
//        {
//            sdaPort = new BiDirectionalPort(pinSda, false, false, ResistorMode.PullUp);
//            sclPort = new BiDirectionalPort(pinScl, false, false, ResistorMode.PullUp);

//        }

//        void Delay (int delayMs)
//        {
//            Thread.Sleep(delayMs);
//        }

//        void Init()
//        {
//            sdaPort.State = false;
//            sclPort.State = false;

//            SetHigh(sdaPort);
//            SetHigh(sclPort);

//            if(sdaPort.State == false || sclPort.State == false)
//            {
//                Console.WriteLine("Error: one port is low");
//            }
//        }

//        public void Start (byte address)
//        {
//            SetLow(sdaPort);
//            Delay(DELAY);
//            SetLow(sclPort);
//            WriteByte(address);
//        }

//        public void Stop ()
//        {
//            SetLow(sdaPort);
//            Delay(DELAY);
//            SetHigh(sclPort);
//            Delay(DELAY);
//            SetHigh(sdaPort);
//            Delay(DELAY);
//        }

//        public void WriteByte(byte value)
//        {
//            for (byte curr = 0x80; curr != 0; curr >>= 1)
//            {
//                if ((curr & value) > 0)
//                    SetHigh(sdaPort);
//                else
//                    SetLow(sdaPort);

//                SetHigh(sclPort);
//                Delay(DELAY);
//                SetLow(sclPort);
//            }

//            //get ack or nak
//            SetHigh(sdaPort);
//            SetHigh(sclPort);
//            Delay(DELAY / 2);
//            bool ack = sdaPort.State;
//            SetLow(sclPort);
//            Delay(DELAY / 2);
//            SetLow(sdaPort);
//        }

//        public byte ReadByte(bool last = false)
//        {
//            byte value = 0;
//            SetHigh(sdaPort);

//            for (byte i = 0; i < 8; i++)
//            {
//                value <<= 1;
//                Delay(DELAY);
//                SetHigh(sclPort);

//                if (sdaPort.State == true)
//                    value |= 1;

//                SetLow(sclPort);
//            }

//            if (last)
//                SetHigh(sdaPort);
//            else
//                SetLow(sdaPort);
//            SetHigh(sclPort);
//            Delay(DELAY / 2);
//            SetLow(sclPort);
//            Delay(DELAY / 2);
//            SetLow(sdaPort);

//            return value;
//        }

//        void SetLow(BiDirectionalPort port)
//        {
//            port.InterrupEnabled = false;
//            port.State = false;
//            port.Active = true;
//            port.InterrupEnabled = true;
//        }

//        void SetHigh(BiDirectionalPort port)
//        {
//            port.InterrupEnabled = false;
//            port.State = true;
//            port.Active = true;
//            port.InterrupEnabled = true;
//        }

//        public void WriteBytes(byte[] values)
//        {
//            for (int i = 0; i < values.Length; i++)
//            {
//                WriteByte(values[i]);
//            }
//        }

//        public void WriteUShort(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
//        {
//            throw new NotImplementedException();
//        }

//        public void WriteUShorts(byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian)
//        {
//            throw new NotImplementedException();
//        }

//        public void WriteRegister(byte address, byte value)
//        {
//            throw new NotImplementedException();
//        }

//        public void WriteRegisters(byte address, byte[] data)
//        {
//            throw new NotImplementedException();
//        }

//        public byte[] WriteRead(byte[] write, ushort length)
//        {
//            throw new NotImplementedException();
//        }

//        public byte[] ReadBytes(ushort numberOfBytes)
//        {
//            byte[] value = new byte[numberOfBytes];
//            for (int i = 0; i < numberOfBytes - 1; i++)
//            {
//                value[i] = ReadByte();
//            }
//            value[numberOfBytes - 1] = ReadByte(true);
//            return value;
//        }

//        public byte ReadRegister(byte address)
//        {
//            throw new NotImplementedException();
//        }

//        public byte[] ReadRegisters(byte address, ushort length)
//        {
//            throw new NotImplementedException();
//        }

//        public ushort ReadUShort(byte address, ByteOrder order)
//        {
//            throw new NotImplementedException();
//        }

//        public ushort[] ReadUShorts(byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}