using Atem.Core.State;

namespace Atem.Core.Graphics.Interrupts
{
    public interface IStatInterruptManager : IStateful
    {
        public bool InterruptOnOAM { get; set; }
        public bool InterruptOnVerticalBlank { get; set; }
        public bool InterruptOnHorizontalBlank { get; set; }
        public bool InterruptOnLineY { get; set; }
        public byte LineYToCompare { get; set; }
        public bool CurrentlyOnLineY { get; }
        public void UpdateLineYCompare();
    }
}
