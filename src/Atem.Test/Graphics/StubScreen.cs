using Atem.Graphics;

namespace Atem.Test.Graphics
{
    internal class StubScreen : IScreen
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float SizeFactor { get; set; }
    }
}
