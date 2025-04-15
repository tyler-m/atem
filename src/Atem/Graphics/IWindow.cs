using System;

namespace Atem.Graphics
{
    public interface IWindow
    {
        public event EventHandler<WindowSizeChangedEventArgs> WindowSizeChanged;
        public int Width { get; }
        public int Height { get; }
        public void SetSize(int width, int height);
    }
}
