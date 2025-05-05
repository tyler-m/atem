using System;
using Atem.Core.State;

namespace Atem.Core.Graphics.Timing
{
    public interface IRenderModeScheduler : IStateful
    {
        public event EventHandler<RenderModeChangedEventArgs> RenderModeChanged;
        public byte CurrentLine { get; }
        public RenderMode Mode { get; }
        public void Clock();
        public void Stop();
        public void Resume();
    }
}
