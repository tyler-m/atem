
using Microsoft.Xna.Framework.Graphics;

namespace Atem.Views.MonoGame
{
    public delegate void OnScreenTextureCreatedEvent(Texture2D texture);

    public interface IScreen
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Texture2D Texture { get; }
        public float SizeFactor { get; set; }
        public event OnScreenTextureCreatedEvent OnScreenTextureCreated;
    }
}
