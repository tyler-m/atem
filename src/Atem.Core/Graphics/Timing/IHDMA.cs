using Atem.Core.State;

namespace Atem.Core.Graphics.Timing
{
    public interface IHDMA : IStateful
    {
        public bool TransferActive { get; set; }
        public ushort SourceAddress { get; set; }
        public ushort DestAddress { get; set; }
        public byte TransferLengthRemaining { get; set; }
        public void ClockTransfer();
        public void StartTransfer(byte value);
    }
}
