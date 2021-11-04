//using System;
//namespace Meadow
//{
//    public static partial class MeadowOS
//    {
//        public static class SystemInformation
//        {
//            public static string OSVersion { get; set; } = "";
//            public static DateTime OSBuildDate { get; set; }
//            public static string MonoVersion { get; set; } = "";


//        }

//        // TODO: move this to the device class and make it a singleton or something
//        public class DeviceInformation
//        {
//            public string DeviceName { get; set; } = "";
//            public HardwareModel Model { get; }
//            public string ProcessorType { get; }
//            public string ProcessorSerialNumber { get; }
//            public string ID { get; }
//            public string CoprocessorType { get; }
//            public string CoprocessorOSVersion { get; }
//            // what about coprocessor build date
//        }

//        /// <summary>
//        /// Hardware version (product).
//        /// </summary>
//        public enum HardwareModel
//        {
//            Unknown = 0,
//            MeadowF7v1 = 1,
//            MeadowF7v2 = 2,
//            MeadowF7v2_Core = 3,
//        };

//    }
//}
