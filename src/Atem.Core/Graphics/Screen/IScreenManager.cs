using Atem.Core.Graphics.Palettes;
using Atem.Core.State;

namespace Atem.Core.Graphics.Screen
{
    public interface IScreenManager : IStateful
    {
        public bool WindowEnabled { get; set; }
        public bool BackgroundAndWindowEnabledOrPriority { get; set; }
        public GBColor[] Screen { get; }
        public void Clock();
    }
}
