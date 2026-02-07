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

        private String nvidiaSMIDirectory = "C:\\Windows\\system32\\nvidia-smi.exe";
        private int PowerLimit;

        // Create object for nvidiaSMI

        private readonly ProcessStartInfo InitSMI;

        // Create constructor to reference instance fields

        // predetermined arguments
        private String ArgPowerLimit = String.Empty;
        private String ArgTDP = String.Empty;

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
            ArgTDP = "-pl";
        } 

        public void VerifyNvidiaSMI()
        {
            DebugOptions debugOptions = new DebugOptions();

            if(debugOptions.CheckFile(nvidiaSMIDirectory))
            {
                debugOptions.WriteLog("Nvidia SMI Exists");
            }
        }

        public int RetrievePowerLimit()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = nvidiaSMIDirectory,
                Arguments = "-q -d POWER", // example argument to show help
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string ConsoleResult = reader.ReadToEnd();

                        // filter out the nvidia smi output to only show the wattage.
                        string[] resultLines = ConsoleResult.Split('\n');

                        foreach (string output in resultLines)
                        {
                            if (output.ToLower().Contains("current power limit") && !output.Contains("N/A"))
                            {
                                String TempResult = output.Split(':')[1].Trim();
                                PowerLimit = Convert.ToInt32(TempResult); // convert the current code to simply output a number.
                                Debug.WriteLine("Current Extract Power Limit: " + PowerLimit);
                                return PowerLimit;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error starting NVIDIA SMI: " + ex.Message);
                MessageBox.Show("Something went wrong. Provide the logs to the github.");
            }

            return 0; // fallback output of 0 if the method fails to return and capture any wattage.
        }

        public bool isTDPModifiable()
        {
            string args = "-pl " + Convert.ToString(RetrievePowerLimit());
            ProcessStartInfo instance = new ProcessStartInfo
            {
                FileName = nvidiaSMIDirectory,
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(instance))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string ConsoleResult = reader.ReadToEnd();

                        Debug.WriteLine("Console output: " +  ConsoleResult);
                        string[] resultLines = ConsoleResult.Split('\n');

                        foreach (string output in resultLines)
                        {
                            if (output.ToLower().Contains("not supported") || output.ToLower().Contains("00000000:01:00.0.")) {
                                return false; // any match above means the hardware is bios locked and can't modify the TDP.
                            } else
                            {
                                return true;
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                Debug.WriteLine("Something unexpected happened");
                MessageBox.Show("Somethign went wrong");
            }

            return false; // fallback to false just to be safe if something wrong happened in the code itself.
        }
    }


}
