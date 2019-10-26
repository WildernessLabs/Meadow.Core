using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            public enum ErrorCode
            {
                OperationNotPermitted = 1,
                NoSuchFileOrDirectory = 2,
                NoSuchProcess = 3,
                InterruptedSystemCall = 4,
                IOError = 5,
                NoSuchDeviceOrAddress = 6,
                ArgListTooLong = 7,
                ExecFormatError = 8,
                BadFileNumber = 9,
                NoChildProcess = 10,
                TryAgain = 11,
                OutOfMemory = 12,
                PermissionDenied = 13,
                BadAddress = 14,
                BlockDevieRequired = 15,
                DeviceOrResourceBusy = 16,
                FileExists = 17,
                CrossDeviceLink = 18,
                NoSuchDevice = 19,
                NotADirectory = 20,
                IsADirectory = 21,
                InvalidArgument = 22,

                //#define ENFILE          23      /* File table overflow */
                //#define EMFILE          24      /* Too many open files */
                //#define ENOTTY          25      /* Not a typewriter */
                //#define ETXTBSY         26      /* Text file busy */
                //#define EFBIG           27      /* File too large */
                //#define ENOSPC          28      /* No space left on device */
                //#define ESPIPE          29      /* Illegal seek */
                //#define EROFS           30      /* Read-only file system */
                //#define EMLINK          31      /* Too many links */
                //#define EPIPE           32      /* Broken pipe */
                //#define EDOM            33      /* Math argument out of domain of func */
                //#define ERANGE          34      /* Math result not representable */
                //#define EDEADLK         35      /* Resource deadlock would occur */
                //#define ENAMETOOLONG    36      /* File name too long */
                //#define ENOLCK          37      /* No record locks available */
                //#define ENOSYS          38      /* Function not implemented */
                //#define ENOTEMPTY       39      /* Directory not empty */
                //#define ELOOP           40      /* Too many symbolic links encountered */
                //#define EWOULDBLOCK     EAGAIN  /* Operation would block */
                //#define ENOMSG          42      /* No message of desired type */
                //#define EIDRM           43      /* Identifier removed */
                //#define ECHRNG          44      /* Channel number out of range */
                //#define EL2NSYNC        45      /* Level 2 not synchronized */
                //#define EL3HLT          46      /* Level 3 halted */
                //#define EL3RST          47      /* Level 3 reset */
                //#define ELNRNG          48      /* Link number out of range */
                //#define EUNATCH         49      /* Protocol driver not attached */
                //#define ENOCSI          50      /* No CSI structure available */
                //#define EL2HLT          51      /* Level 2 halted */
                //#define EBADE           52      /* Invalid exchange */
                //#define EBADR           53      /* Invalid request descriptor */
                //#define EXFULL          54      /* Exchange full */
                //#define ENOANO          55      /* No anode */
                //#define EBADRQC         56      /* Invalid request code */
                //#define EBADSLT         57      /* Invalid slot */

                //#define EDEADLOCK       EDEADLK

                //#define EBFONT          59      /* Bad font file format */
                //#define ENOSTR          60      /* Device not a stream */
                //#define ENODATA         61      /* No data available */
                //#define ETIME           62      /* Timer expired */
                //#define ENOSR           63      /* Out of streams resources */
                //#define ENONET          64      /* Machine is not on the network */
                //#define ENOPKG          65      /* Package not installed */
                //#define EREMOTE         66      /* Object is remote */
                //#define ENOLINK         67      /* Link has been severed */
                //#define EADV            68      /* Advertise error */
                //#define ESRMNT          69      /* Srmount error */
                //#define ECOMM           70      /* Communication error on send */
                //#define EPROTO          71      /* Protocol error */
                //#define EMULTIHOP       72      /* Multihop attempted */
                //#define EDOTDOT         73      /* RFS specific error */
                //#define EBADMSG         74      /* Not a data message */
                //#define EOVERFLOW       75      /* Value too large for defined data type */
                //#define ENOTUNIQ        76      /* Name not unique on network */
                //#define EBADFD          77      /* File descriptor in bad state */
                //#define EREMCHG         78      /* Remote address changed */
                //#define ELIBACC         79      /* Can not access a needed shared library */
                //#define ELIBBAD         80      /* Accessing a corrupted shared library */
                //#define ELIBSCN         81      /* .lib section in a.out corrupted */
                //#define ELIBMAX         82      /* Attempting to link in too many shared libraries */
                //#define ELIBEXEC        83      /* Cannot exec a shared library directly */
                //#define EILSEQ          84      /* Illegal byte sequence */
                //#define ERESTART        85      /* Interrupted system call should be restarted */
                //#define ESTRPIPE        86      /* Streams pipe error */
                //#define EUSERS          87      /* Too many users */
                //#define ENOTSOCK        88      /* Socket operation on non-socket */
                //#define EDESTADDRREQ    89      /* Destination address required */
                //#define EMSGSIZE        90      /* Message too long */
                //#define EPROTOTYPE      91      /* Protocol wrong type for socket */
                //#define ENOPROTOOPT     92      /* Protocol not available */
                //#define EPROTONOSUPPORT 93      /* Protocol not supported */
                //#define ESOCKTNOSUPPORT 94      /* Socket type not supported */
                //#define EOPNOTSUPP      95      /* Operation not supported on transport endpoint */
                //#define EPFNOSUPPORT    96      /* Protocol family not supported */
                //#define EAFNOSUPPORT    97      /* Address family not supported by protocol */
                //#define EADDRINUSE      98      /* Address already in use */
                //#define EADDRNOTAVAIL   99      /* Cannot assign requested address */
                //#define ENETDOWN        100     /* Network is down */
                //#define ENETUNREACH     101     /* Network is unreachable */
                //#define ENETRESET       102     /* Network dropped connection because of reset */
                //#define ECONNABORTED    103     /* Software caused connection abort */
                //#define ECONNRESET      104     /* Connection reset by peer */
                //#define ENOBUFS         105     /* No buffer space available */
                //#define EISCONN         106     /* Transport endpoint is already connected */
                //#define ENOTCONN        107     /* Transport endpoint is not connected */
                //#define ESHUTDOWN       108     /* Cannot send after transport endpoint shutdown */
                //#define ETOOMANYREFS    109     /* Too many references: cannot splice */
                //#define ETIMEDOUT       110     /* Connection timed out */
                //#define ECONNREFUSED    111     /* Connection refused */
                //#define EHOSTDOWN       112     /* Host is down */
                //#define EHOSTUNREACH    113     /* No route to host */
                //#define EALREADY        114     /* Operation already in progress */
                //#define EINPROGRESS     115     /* Operation now in progress */
                //#define ESTALE          116     /* Stale NFS file handle */
                //#define EUCLEAN         117     /* Structure needs cleaning */
                //#define ENOTNAM         118     /* Not a XENIX named type file */
                //#define ENAVAIL         119     /* No XENIX semaphores available */
                //#define EISNAM          120     /* Is a named type file */
                //#define EREMOTEIO       121     /* Remote I/O error */
                //#define EDQUOT          122     /* Quota exceeded */

                //#define ENOMEDIUM       123     /* No medium found */
                //#define EMEDIUMTYPE     124     /* Wrong medium type */            }
            }

            public enum UpdIoctlFn
            {
                SetRegister = 1,
                GetRegister = 2,
                UpdateRegister = 3,
                RegisterGpioIrq = 5,
                PwmSetup = 10,
                PwmShutdown = 11,
                PwmStart = 12,
                PwmStop = 13,

                I2CShutdown = 20,
                I2CData = 21,

                SPIData = 31,
                SPISpeed = 32,
                SPIMode = 33,

                DirEnum = 41,

                GetLastError = 51
            }

            public struct UpdRegisterValue
            {
                public uint Address;
                public uint Value;
            }

            public struct UpdRegisterUpdate
            {
                public uint Address;
                public uint ClearBits;
                public uint SetBits;
            }

            public struct UpdGpioInterruptConfiguration
            {
                public int Irq;
                public int Port;
                public int Pin;
                public bool Enable;
                public bool RisingEdge;
                public bool FallingEdge;
            }

            public struct UpdI2CCommand
            {
                public int Address;
                public int Frequency;
                public IntPtr TxBuffer;
                public int TxBufferLength;
                public IntPtr RxBuffer;
                public int RxBufferLength;

            }

            public struct UpdSPIDataCommand
            {
                public IntPtr TxBuffer;
                public IntPtr RxBuffer;
                /// <summary>
                /// SPI requires both buffers be equal length
                /// </summary>
                public int BufferLength;
                public int BusNumber;
            }

            public struct UpdSPISpeedCommand
            {
                public int BusNumber;
                /// <summary>
                /// Raw frequency, in Hz
                /// </summary>
                /// <remarks>
                /// STM32 Supports the following:
                /// 48000000,
                /// 24000000,
                /// 12000000,
                /// 6000000,
                /// 3000000,
                /// 1500000,
                /// 750000,
                /// 375000
                /// </remarks>
                public long Frequency;
            }

            public struct UpdSPIModeCommand
            {
                public int BusNumber;
                public int Mode;
            }

            public struct UpdPwmCmd
            {
                public uint Timer;
                public uint Channel;
                // Members below only applicable for PwmStart cmd.
                public uint Frequency;
                public uint Duty;
            }

            /*
             struct upd_dir_enum_cmd
             {
                char* root,
                char* result,
                uint32_t resultlength
             }
            */
            public struct UpdEnumDirCmd
            {
                public IntPtr RootFolder;
                public IntPtr Buffer;
                public int BufferLength;
            }

            /*
            struct gpio_int_config
            {
              uint32_t irq;
              uint32_t port;
              uint32_t pin;
              bool enable;
              bool risingEdge;
              bool fallingEdge;
            };

            struct upd_config_interrupt
            {
              uint32_t interruptType;

              union cfg
              {
                struct gpio_int_config gpio;
                void *foo;
              } cfg;
            };
            */
            public static bool TryGetRegister(IntPtr driverHandle, int address, out uint value)
            {
                return TryGetRegister(driverHandle, (uint)address, out value);
            }

            public static bool TryGetRegister(IntPtr driverHandle, uint address, out uint value)
            {
                var register = new UpdRegisterValue
                {
                    Address = address
                };
                //                Console.WriteLine($"Reading register: {register.Address:X}");
                var result = Interop.Nuttx.ioctl(driverHandle, UpdIoctlFn.GetRegister, ref register);
                if (result != 0)
                {
                    Console.WriteLine($"Read failed: {result}");
                    value = (uint)result;
                    return false;
                }
//                Console.WriteLine($"Value: {register.Value:X}");
                value = register.Value;
                return true;
            }

            public static bool SetRegister(IntPtr driverHandle, int address, uint value)
            {
                return SetRegister(driverHandle, (uint)address, value);
            }

            public static bool SetRegister(IntPtr driverHandle, uint address, uint value)
            {
                var register = new UpdRegisterValue
                {
                    Address = address,
                    Value = value
                };
                //                Console.WriteLine($"Writing {register.Value:X} to register: {register.Address:X}");
                var result = Interop.Nuttx.ioctl(driverHandle, UpdIoctlFn.SetRegister, ref register);
                if (result != 0)
                {
                    Console.WriteLine($"Write failed: {result}");
                    return false;
                }
                return true;

            }

            public static bool UpdateRegister(IntPtr driverHandle, int address, uint clearBits, uint setBits)
            {
                return UpdateRegister(driverHandle, (uint)address, clearBits, setBits);
            }

            public static bool UpdateRegister(IntPtr driverHandle, uint address, uint clearBits, uint setBits)
            {
                var update = new UpdRegisterUpdate()
                {
                    Address = address,
                    ClearBits = clearBits,
                    SetBits = setBits
                };
//                Console.WriteLine($"Updating register: {update.Address:X} clearing {update.ClearBits:X} setting {update.SetBits:X}");
                var result = Interop.Nuttx.ioctl(driverHandle, UpdIoctlFn.UpdateRegister, ref update);
                if (result != 0)
                {
                    Console.WriteLine($"Update failed: {result}");
                    return false;
                }
                return true;

            }

        }
    }
}

