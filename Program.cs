using System;

namespace Atem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Core.Atem atem = new Core.Atem();
            using View view = new View(atem);
            view.Run();
        }
    }
}