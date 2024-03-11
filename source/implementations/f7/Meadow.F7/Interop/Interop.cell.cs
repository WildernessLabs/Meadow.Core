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
        public static extern void meadow_cell_change_state(int s);

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int meadow_get_cell_error();

        public static List<CellNetwork>? ParseCellNetworkScannerOutput(string input)
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
                    mode = modeValue switch
                    {
                        0 => CellNetworkMode.GSM,
                        7 or 8 => CellNetworkMode.CATM1,
                        9 => CellNetworkMode.NBIOT,
                        _ => CellNetworkMode.GSM,
                    };
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
    }
}