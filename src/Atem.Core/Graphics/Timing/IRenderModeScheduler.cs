using Atem.Core.State;
using System;
using System.Xml.Linq;

namespace Atem.Core.Graphics.Timing
{
    public interface IRenderModeScheduler : IStateful
    {
        public event EventHandler<RenderModeChangedEventArgs> RenderModeChanged;
        public byte CurrentLine { get; }
        public RenderMode Mode { get; }
        public void Clock();
    }
}
