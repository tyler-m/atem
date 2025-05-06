using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Atem
{
    public static class NativeLibraryLoader
    {
        private static readonly string _assemblyDirectoryPath = Path.GetDirectoryName(AppContext.BaseDirectory);

        private static readonly Dictionary<OSPlatform, string> PlatformId = new()
        {
            { OSPlatform.Windows, "win-x64" },
            { OSPlatform.Linux, "linux-x64" },
        };

        private static readonly Dictionary<OSPlatform, string[]> PlatformLibraries = new()
        {
            { OSPlatform.Windows, ["cimgui.dll"] },
            { OSPlatform.Linux, ["libcimgui.so"] }
        };

        public static void LoadLibraries()
        {
            foreach ((OSPlatform platform, string[] libraries) in PlatformLibraries)
            {
                if (RuntimeInformation.IsOSPlatform(platform))
                {
                    LoadPlatformLibraries(platform, libraries);
                    break;
                }
            }
        }

        private static void LoadPlatformLibraries(OSPlatform platform, string[] libraries)
        {
            if (!PlatformId.TryGetValue(platform, out string systemId))
            {
                throw new Exception($"No system ID defined for platform {platform}.");
            }

            foreach (string library in libraries)
            {
                string libraryFilePath = Path.Combine(_assemblyDirectoryPath, "runtimes", systemId, "native", library);

                if (File.Exists(libraryFilePath))
                {
                    NativeLibrary.Load(libraryFilePath);
                }
                else
                {
                    throw new Exception($"Native library not found at {libraryFilePath}");
                }
            }
        }
    }
}
