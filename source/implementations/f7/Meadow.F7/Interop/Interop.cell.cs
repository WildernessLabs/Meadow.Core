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
        public static extern int meadow_cell_change_state(int state, IntPtr data);

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int meadow_get_cell_error();

        /// <summary>
        /// Changes the state of the cell using the specified configuration object.
        /// </summary>
        /// <param name="state">The state of the cell.</param>
        /// <param name="config">The configuration object for the cell state change.</param>
        /// <returns>The result of the state change operation.</returns>
        public static void ChangeCellState(int state, CellStateConfig config)
        {
            // Check if the config is null
            IntPtr configPtr = IntPtr.Zero;
            try
            {
                if (config != null)
                {
                    Resolver.Log.Trace($"Preparing to change cell state to {state} with config: {config}");

                    int configSize = Marshal.SizeOf(config);
                    configPtr = Marshal.AllocHGlobal(configSize);
                    Marshal.StructureToPtr(config, configPtr, false);

                    Resolver.Log.Trace($"State: {state}, Config Size: {configSize}, ConfigPtr: {configPtr}");
                }
                else
                {
                    Resolver.Log.Trace($"Preparing to change cell state to {state} with no config.");
                }

                int result = Core.Interop.Nuttx.meadow_cell_change_state(state, configPtr);

                if (result != 0)
                {
                    Resolver.Log.Error($"Failed to change cellular state with error code: {result}");
                    throw new Exception($"Failed to change cellular state with error code: {result}");
                }
                else
                {
                    Resolver.Log.Trace($"Successfully changed cellular state to {state}");
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Error changing cell state: {ex.Message}");
                throw;
            }
            finally
            {
                if (configPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(configPtr);
                    Resolver.Log.Trace($"Freed memory for configPtr: {configPtr}");
                }
            }
        }

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