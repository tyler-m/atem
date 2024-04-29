using Atem.Core;
using Atem.Views.MonoGame;

namespace Atem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AtemRunner atem = new AtemRunner();
            using View view = new View(atem);
            view.Run();
        }
    }
}