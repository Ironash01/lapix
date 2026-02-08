using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Drawing.Text;
using LapixModules.debug; // use for process start and debug output

namespace LapixModules.nsmi_mod
{

    public class NSMIWrapper
    {
        //nvidia smi util directory
        //change if user has customized drivers or other gpus.
        //currently only nvidia cards are supported desktop and mobile.

        readonly private String nvidiaSMIDirectory = "C:\\Windows\\system32\\nvidia-smi.exe";
        private int PowerLimit;

        // Create object for nvidiaSMI

        private readonly ProcessStartInfo InitSMI;

        // Create constructor to reference instance fields

        // predetermined arguments
        readonly private String ArgPowerLimit = String.Empty;
        readonly private String ArgTDP = String.Empty;
        readonly private String ArgSupportedClocks = String.Empty;

        DebugOptions debugOptions = new DebugOptions();

        public NSMIWrapper()
        {
            InitSMI = new ProcessStartInfo
            {
                FileName = nvidiaSMIDirectory,
                Arguments = "-0", // dummy arg to trigger not recognized if args is passed incorrectly.
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Define Args CheatSheet Here.

            ArgPowerLimit = "-q -d POWER";
            ArgTDP = "-pl ";
            ArgSupportedClocks = "-q -d SUPPORTED_CLOCKS";
        }

        public void VerifyNvidiaSMI()
        {

            if (debugOptions.CheckFile(nvidiaSMIDirectory))
            {
                debugOptions.WriteLog("Nvidia SMI Exists");
            }
        }

        public int GetPowerLimit()
        {
            InitSMI.Arguments = ArgPowerLimit;
            Process? process = null;

            if (InitSMI != null)
            {
                process = Process.Start(InitSMI);
            }
            else
            {
                debugOptions.WriteLog("Process instance is null, something went wrong with the code.");
                return 0;
            }
            try
            {
                if (process != null)
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string ConsoleResult = reader.ReadToEnd();
                        //debugOptions.WriteLog("Console output: " + ConsoleResult);
                        string[] ConsoleResultArray = ConsoleResult.Split('\n');

                        foreach (string output in ConsoleResultArray)
                        {
                            if (output.ToLower().Contains("current power limit") && !output.Contains("N/A"))
                            {
                                //debugOptions.WriteLog("Current Power Limit Line: " + output.Split(':')[1].Trim());
                                String PL = output.Split(':')[1].Trim();
                                PL = PL.Replace(" W", " ");
                                double PowerLimitDouble = Convert.ToDouble(PL);
                                PowerLimit = (int)PowerLimitDouble;
                                debugOptions.WriteLog("Current Power Limit: " + PowerLimit + "W");
                                return PowerLimit;
                            }
                        }
                    }
                }
                else
                {
                    debugOptions.WriteLog("Process instance is null, something went wrong with the code.");
                    return 0; // fallback to 0 if something went wrong, should be handled better in the future.
                }
            }
            catch (Exception ex)
            {
                debugOptions.WriteLog("Something unexpected happened: " + ex.Message);
                return 0;

            }
            finally
            {
                if (process != null && !process.HasExited)
                {
                    process.Dispose();
                }
            }

            return 0;

        }

        public bool TDPModifiable()
        {
            InitSMI.Arguments = ArgTDP + this.GetPowerLimit();
            debugOptions.WriteLog("Checking TDP Modifiability with Argument: " + InitSMI.Arguments);
            Process? process = null;

            if (InitSMI != null)
            {
                process = Process.Start(InitSMI);
            }
            else
            {
                debugOptions.WriteLog("Process instance is null, something went wrong with the code.");
                return false;
            }

            try
            {
                if (process != null)
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string ConsoleResult = reader.ReadToEnd();
                        debugOptions.WriteLog(ConsoleResult);

                        string[] resultLines = ConsoleResult.Split('\n');

                        foreach (string line in resultLines)
                        {
                            String output = line.ToLower();
                            if (output.Contains("not supported") || output.Contains("00000000:01:00.0.") || output.Contains("insufficient permissions"))
                            {
                                debugOptions.WriteLog("TDP Modifiable: False, reason: " + output.Trim());
                                return false; // any match above means the hardware is bios locked and can't modify the TDP.
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    debugOptions.WriteLog("Process instance is null, something went wrong with the code.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                debugOptions.WriteLog("Something unexpected happened");
            }
            finally
            {
                if (process != null && !process.HasExited)
                {
                    process.Dispose();
                }
            }

            return true; // fallback to false just to be safe if something wrong happened in the code itself.
        }

        public List<int> GetSupportedClocks()
        {
            List<int> SupportedClocks = new List<int>();
            InitSMI.Arguments = ArgSupportedClocks;
            Process? process = null;
            if (InitSMI != null)
            {
                process = Process.Start(InitSMI);
            }
            else
            {
                debugOptions.WriteLog("Process instance is null, something went wrong with the code.");
                return SupportedClocks;
            }
            try
            {
                if (process != null)
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string ConsoleResult = reader.ReadToEnd();
                        debugOptions.WriteLog(ConsoleResult);
                        string[] resultLines = ConsoleResult.Split('\n');
                        foreach (string line in resultLines)
                        {
                            String output = line.ToLower();
                            if (output.Contains("graphics") && output.Contains("mhz"))
                            {
                                String ClockString = output.Split(':')[1].Trim();
                                debugOptions.WriteLog("Found Clock Line: " + ClockString);
                                ClockString = ClockString.Replace(" mhz", " ");
                                int ClockInt = Convert.ToInt32(ClockString);
                                SupportedClocks.Add(ClockInt);
                                debugOptions.WriteLog("Found Supported Clock: " + ClockInt + "MHz");
                            }
                        }
                    }
                }
                else
                {
                    debugOptions.WriteLog("Process instance is null, something went wrong with the code.");
                    return SupportedClocks;
                }
            }
            catch (Exception ex)
            {
                debugOptions.WriteLog("Something unexpected happened: " + ex.Message);
                return SupportedClocks;
            }
            finally
            {
                if (process != null && !process.HasExited)
                {
                    process.Dispose();
                }
            }

            SupportedClocks.Sort();
            return SupportedClocks;

        }

        public List<int> GetSupportedMemoryClocks()
        {
            List<int> SupportedMemoryClocks = new List<int>();
            InitSMI.Arguments = ArgSupportedClocks;
            Process? process = null;
            if (InitSMI != null)
            {
                process = Process.Start(InitSMI);
            }
            else
            {
                debugOptions.WriteLog("Process instance is null, something went wrong with the code.");
                return SupportedMemoryClocks;
            }
            try
            {
                if (process != null)
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string ConsoleResult = reader.ReadToEnd();
                        //debugOptions.WriteLog(ConsoleResult);
                        string[] resultLines = ConsoleResult.Split('\n');
                        foreach (string line in resultLines)
                        {
                            String output = line.ToLower();
                            if (output.Contains("memory") && output.Contains("mhz"))
                            {
                                String ClockString = output.Split(':')[1].Trim();
                                //debugOptions.WriteLog("Found Memory Clock Line: " + ClockString);
                                ClockString = ClockString.Replace(" mhz", " ");
                                int ClockInt = Convert.ToInt32(ClockString);
                                SupportedMemoryClocks.Add(ClockInt);
                                debugOptions.WriteLog("Found Supported Memory Clock: " + ClockInt + "MHz");
                            }
                        }
                    }
                }
                else
                {
                    debugOptions.WriteLog("Process instance is null, something went wrong with the code.");
                    return SupportedMemoryClocks;
                }
            }
            catch (Exception ex)
            {
                debugOptions.WriteLog("Something unexpected happened: " + ex.Message);
                return SupportedMemoryClocks;
            }
            finally
            {
                if (process != null && !process.HasExited)
                {
                    process.Dispose();
                }
            }
            SupportedMemoryClocks.Sort();
            return SupportedMemoryClocks;
        }
    }
}
