using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualBasic.Logging;
using System.Drawing.Text;

namespace LapixModules.debug
{
    // Start practicing to output debugs on runtime not the project instance.
    public class DebugOptions
    {
        private bool DebugMode;
        private String DebugString = string.Empty;

        private String BaseDirectory = AppContext.BaseDirectory;
        private String DirectoryLog = string.Empty;
        private String FileLog = string.Empty;

        public DebugOptions()
        {
            DebugMode = true;
            DirectoryLog = Path.Combine(BaseDirectory, "logs");
            FileLog = "debug.log";

        }

        public void WriteLog(String LogArgument)
        {
            // add a seperate checker to prevent WriteLog from failing
            Directory.CreateDirectory(DirectoryLog);

            if (!File.Exists(Path.Combine(DirectoryLog, FileLog)))
            {
                File.Create(Path.Combine(DirectoryLog, FileLog)).Close();
            }

            DateTime now = DateTime.Now;

            LogArgument = now.ToString("yyyy-MM-dd HH:mm:ss ") + "DEBUG OUTPUT: " + LogArgument;

            Debug.WriteLine(LogArgument);
            File.AppendAllText(Path.Combine(DirectoryLog, FileLog), LogArgument + Environment.NewLine);
        }

        public void GuaranteeFolder(String FolderArgument)
        {

            if (Directory.Exists(FolderArgument))
            {
                this.WriteLog("Folder Exists: " + FolderArgument);
            }
            else
            {
                this.WriteLog("Folder does not exist, creating folder: " + FolderArgument);
                Directory.CreateDirectory(FolderArgument);
            }
        }

        public void GuaranteeFile(String FileArgument)
        {
            DebugOptions debugOptions = new DebugOptions();

            if (File.Exists(FileArgument))
            {
                debugOptions.WriteLog("File Exists: " + FileArgument);
            }
            else
            {
                debugOptions.WriteLog("File does not exist, creating file: " + FileArgument);
                File.Create(FileArgument).Close();
            }
        }

        public bool CheckFile(String FileArgument)
        {
            DebugOptions debugOptions = new DebugOptions();

            if (File.Exists(FileArgument))
            {
                debugOptions.WriteLog("File Exists: " + FileArgument);
                return true;
            }
            else
            {
                debugOptions.WriteLog("File does not exist: " + FileArgument);
                return false;
            }
        }

        public bool CheckFolder(String FolderArgument)
        {
            DebugOptions debugOptions = new DebugOptions();
            if (Directory.Exists(FolderArgument))
            {
                debugOptions.WriteLog("Folder Exists: " + FolderArgument);
                return true;
            }
            else
            {
                debugOptions.WriteLog("Folder does not exist: " + FolderArgument);
                return false;
            }
        }
    }
}
