using Atem.Core;
using Atem.Factories;
using Atem.Views.MonoGame;

namespace Atem
{
    internal class Program
    {
        static void Main()
        {
            NativeLibraryLoader.LoadLibraries();
            Emulator emulator = EmulatorFactory.Create();
            ViewStarter.Start(emulator);
        }
    }
}