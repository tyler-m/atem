using System;

namespace Atem.Graphics
{
    public class Window : IWindow
    {
        private int _width;
        private int _height;

        public int Width => _width;
        public int Height => _height;

        public event EventHandler<WindowSizeChangedEventArgs> WindowSizeChanged;

        public void SetSize(int newWidth, int newHeight)
        {
            WindowSizeChangedEventArgs args = new(newWidth, newHeight, _width, _height);

            _width = newWidth;
            _height = newHeight;

            WindowSizeChanged?.Invoke(this, args);
        }
    }
}
