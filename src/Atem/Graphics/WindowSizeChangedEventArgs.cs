using System;

namespace Atem.Graphics
{
    public class WindowSizeChangedEventArgs : EventArgs
    {
        public int CurrentWidth { get; }
        public int CurrentHeight { get; }
        public int PreviousWidth { get; }
        public int PreviousHeight { get; }

        public WindowSizeChangedEventArgs(int currentWidth, int currentHeight, int previousWidth, int previousHeight)
        {
            CurrentWidth = currentWidth;
            CurrentHeight = currentHeight;
            PreviousWidth = previousWidth;
            PreviousHeight = previousHeight;
        }
    }
}
