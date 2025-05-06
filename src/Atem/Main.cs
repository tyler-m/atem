using System;
using System.IO;
using System.Runtime.InteropServices;
using Atem.Core;
using Atem.Factories;
using Atem.Views.MonoGame;

namespace Atem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LoadLibraries();
            Start();
        }

        private static void LoadLibraries()
        {
            string assemblyDirectoryPath = Path.GetDirectoryName(AppContext.BaseDirectory);

            string[] libraries = [];
            string systemId = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                libraries = ["cimgui.dll"];
                systemId = "win-x64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                libraries = ["libcimgui.so"];
                systemId = "linux-x64";
            }

            foreach (string library in libraries)
            {
                string libraryFilePath = Path.Combine(assemblyDirectoryPath, "runtimes", systemId, "native", library);

                if (File.Exists(libraryFilePath))
                {
                    NativeLibrary.Load(libraryFilePath);
                }
            }
        }

        private static void Start()
        {
            Emulator emulator = EmulatorFactory.Create();
            ViewStarter.Start(emulator);
        }
    }
}