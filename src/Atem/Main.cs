using Atem.Core;
using Atem.Views.MonoGame;

namespace Atem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Emulator emulator = new();
            ViewStarter.Start(emulator);
        }
    }
}