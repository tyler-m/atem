﻿using Atem.Core.Graphics.Palettes;
using Atem.Core.State;

namespace Atem.Core.Graphics.Screen
{
    public interface IScreenManager : IStateful
    {
        public bool WindowEnabled { get; set; }
        public bool BackgroundAndWindowEnabledOrPriority { get; set; }
        public int ScreenX { get; set; }
        public int ScreenY { get; set; }
        public int WindowX { get; set; }
        public int WindowY { get; set; }
        public GBColor[] Screen { get; set; }
        public void Clock();
    }
}
