using Atem.Graphics;

namespace Atem.Test.Graphics
{
    internal class StubScreen : IScreen
    {
        public bool SizeLocked { get; set; }
        public int SizeFactor { get; set; }
    }
}
