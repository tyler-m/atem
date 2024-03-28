using System;

namespace Atem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Atem atem = new Atem();
            using View view = new View(atem);
            view.Run();
        }
    }
}