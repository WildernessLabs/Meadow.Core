using Meadow.Networking;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
namespace Meadow.Core;

internal static partial class Interop
{
    public static partial class Nuttx
    {
        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern bool meadow_cell_is_connected();

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int meadow_get_cell_at_cmds_output(IntPtr buf);

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int meadow_cell_scanner(IntPtr buf);

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int meadow_get_cell_error();        

        private static List<CellNetwork> Parse(string input)
        {
            if (input.Contains("+CME ERROR"))
            {
                return null;
            }

            string pattern = @"\((\d+),""([^""]+)"",""([^""]+)"",""([^""]+)""(?:,(\d+))?\)";
            List<CellNetwork> cellNetworks = new List<CellNetwork>();
            MatchCollection matches = Regex.Matches(input, pattern);

            // If no +CME ERROR is encountered and no network outputs are present,
            // it indicates that the scanner has reached the Cell connection timeout
            if (matches.Count == 0)
            {
                throw new System.TimeoutException("Timeout reached during cell network scanning.");
            }

            foreach (Match match in matches)
            {
                int status = int.Parse(match.Groups[1].Value);
                string operatorName = (match.Groups[2].Value);
                string operatorAlias = (match.Groups[3].Value);
                string operatorCode = (match.Groups[4].Value);

                CellNetworkMode mode;

                if (int.TryParse(match.Groups[5].Value, out int modeValue))
                {
                    switch (modeValue)
                    {
                        case 0:
                            mode = CellNetworkMode.GSM;
                            break;
                        case 7:
                        case 8:
                            mode = CellNetworkMode.CAT_M1;
                            break;
                        case 9:
                            mode = CellNetworkMode.NB_IoT;
                            break;
                        default:
                            mode = CellNetworkMode.GSM;
                            break;
                    }
                }
                else
                {
                    mode = CellNetworkMode.GSM;
                }

                var network = new CellNetwork
                {
                    Name = operatorName,
                    Operator = operatorAlias,
                    Code = operatorCode,
                    Status = (CellNetworkStatus)status,
                    Mode = mode
                };

                cellNetworks.Add(network);
            }

            return cellNetworks;
        }

        public static unsafe CellNetwork[] MeadowCellNetworkScanner()
        {
            var buffer = Marshal.AllocHGlobal(1024);

            try
            {
                Resolver.Log.Info("Scanning cell networks...");
                var len = meadow_cell_scanner(buffer);
                if (len > 0)
                {
                    var data = Encoding.UTF8.GetString((byte*)buffer.ToPointer(), len);

                    return Parse(data).ToArray();
                }
                else
                {
                    throw new System.IO.IOException("No available networks found, please ensure that your device is in scanning mode.");
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"No available networks found: {ex.Message}");
                return Array.Empty<CellNetwork>();
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
