using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System;

using Meadow.Networking;
namespace Meadow.Core;

internal static partial class Interop
{
    internal  enum CellNetworkMode { GSM =0,CAT_M1_BG77 = 7,CAT_M1 = 8, NB_IoT = 9  };
    public static partial class Nuttx
    {
        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern bool meadow_cell_is_connected();

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int meadow_get_cell_pppd_output(IntPtr buf);  

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int meadow_cell_scanner(IntPtr buf);
        
        private static string GetModeNetwork(CellNetworkMode mode)
        {
            CellNetworkMode cellMode = (CellNetworkMode)mode;
                    switch (cellMode)
                    {
                        case CellNetworkMode.GSM:
                            return  "GSM";
                        
                        case CellNetworkMode.CAT_M1_BG77:
                            return  "Cat-M1";
                        
                        case CellNetworkMode.CAT_M1:
                            return   "Cat-M1";
                        
                        case CellNetworkMode.NB_IoT:
                           return  "NB-IoT";
                        
                        default:
                            return  "Unknown";
                        
                    }
        }
        private static List<CellNetwork> Parse (string input)
        {
            if(input.Contains("+CME ERROR"))
            {
                return null;
            }
            string pattern = @"\((\d+),""([^""]+)"",""([^""]+)"",""([^""]+)""(?:,(\d+))?\)";
            List<CellNetwork> cellNetworks = new List<CellNetwork>();
            MatchCollection matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                int index = int.Parse(match.Groups[1].Value);
                string operatorName = match.Groups[2].Value;
                string _operator = (match.Groups[3].Value);
                string operetorCode = (match.Groups[4].Value);
                
                string? mode;

                if (int.TryParse(match.Groups[5].Value, out int modeValue)&& Enum.IsDefined(typeof(CellNetworkMode), modeValue))
                {
                   mode = GetModeNetwork((CellNetworkMode)modeValue);
                }
                else
                {
                    mode = "GSM";
                }
                
                cellNetworks.Add(new CellNetwork {
                                                    Index    = index, 
                                                    Name     = operatorName,
                                                    Operator =_operator,
                                                    Code     = operetorCode,
                                                    Mode     = mode,
                                                 });

            } 
            return cellNetworks;
        }

        public static unsafe List <CellNetwork> MeadowCellScannerNetwork()
        {
            var buffer = Marshal.AllocHGlobal(1024);

            try
            {
                var len  = meadow_cell_scanner(buffer);
                if(len > 0)
                {
                    var data = Encoding.UTF8.GetString((byte*)buffer.ToPointer(), len);
                    
                    return Parse(data);
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                // free resources. this will always run
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
