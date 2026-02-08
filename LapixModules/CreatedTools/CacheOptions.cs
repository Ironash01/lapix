using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LapixModules.debug;

namespace LapixModules.CreatedTools
{
    public class CacheOptions
    {
        DebugOptions debugOptions = new();

        private string GPUCacheDir;
        private string GPUCacheFile;

        public CacheOptions()
        {
            GPUCacheDir = Path.Combine(AppContext.BaseDirectory, "GPUCache");
            GPUCacheFile = Path.Combine(GPUCacheDir, "GPUInfoCache.json");
        }
        public void StoreGPUInfo()
        {
            debugOptions.GuaranteeFolder(GPUCacheDir);

            if (debugOptions.CheckFile(GPUCacheFile))
            {
                debugOptions.WriteLog("GPU Cache file already exists, skipping GPU info retrieval.");
                return;
            } else
            {
                debugOptions.GuaranteeFile(GPUCacheFile);
            }

            using (StreamWriter writer = new StreamWriter(GPUCacheFile))
            {
                // Simulate GPU info retrieval and caching
                writer.WriteLine();
            }

        }
    }
}
