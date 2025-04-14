using System.IO;
using Atem.Core.State;

namespace Atem.Core.Graphics.Timing
{
    /// <summary>
    /// Implements Horizontal-Blank Direct Memory Access (HDMA) for the Game Boy Color.
    /// HDMA allows data to be transferred from ROM or RAM to VRAM during the H-Blank
    /// rendering period between scanlines without hogging the CPU. This is used by
    /// games to dynamically update tiles or palettes during frame rendering.
    public class HDMA : IStateful
    {
        private readonly IBus _bus;
        private bool _justEnteredHorizontalBlank;
        private bool _horizontalBlankTransfer;
        private bool _horizontalBlankTransferActive;
        private bool _transferActive;
        private ushort _sourceAddress;
        private ushort _destAddress;
        private byte _transferLengthRemaining;

        public bool JustEnteredHorizontalBlank { get => _justEnteredHorizontalBlank; set => _justEnteredHorizontalBlank = value; }
        public bool TransferActive { get => _transferActive; set => _transferActive = value; }
        public ushort SourceAddress { get => _sourceAddress; set => _sourceAddress = value; }
        public ushort DestAddress { get => _destAddress; set => _destAddress = value; }
        public byte TransferLengthRemaining { get => _transferLengthRemaining; set => _transferLengthRemaining = value; }

        public HDMA(IBus bus)
        {
            _bus = bus;
        }

        public void ClockTransfer()
        {
            _horizontalBlankTransferActive = _transferActive && _justEnteredHorizontalBlank && _horizontalBlankTransfer;

            // this condition is satisfied only once per horizontal blank
            if (_horizontalBlankTransferActive)
            {
                for (int i = 0; i < 16; i++)
                {
                    _bus.Graphics.WriteVRAM(_destAddress, _bus.Read(_sourceAddress), true);
                    _destAddress++;
                    _sourceAddress++;
                }

                _transferLengthRemaining--;
                _horizontalBlankTransferActive = false;

                if (_transferLengthRemaining == 0xFF)
                {
                    _transferActive = false;
                }
            }

            if (_justEnteredHorizontalBlank)
            {
                _justEnteredHorizontalBlank = false;
            }
        }

        public void StartTransfer(byte value)
        {
            if (_transferActive && !value.GetBit(7))
            {
                // cancel current horizontal blank transfer
                _transferActive = false;
            }
            else
            {
                _horizontalBlankTransfer = value.GetBit(7);

                // number of bytes to transfer divided by 16, minus 1
                // e.g. a length of 0 means 16 bytes to transfer
                _transferLengthRemaining = (byte)(value & 0x7F);

                if (_horizontalBlankTransfer)
                {
                    _transferActive = true;
                }
                else
                {
                    // because we aren't waiting for horizontal blank
                    // we fulfill the transfer request in-place
                    while (_transferLengthRemaining != 0xFF)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            _bus.Graphics.WriteVRAM(_destAddress++, _bus.Read(_sourceAddress++), true);
                        }

                        _transferLengthRemaining--;
                    }
                }
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_sourceAddress);
            writer.Write(_destAddress);
            writer.Write(_transferLengthRemaining);
            writer.Write(_transferActive);
            writer.Write(_horizontalBlankTransferActive);
            writer.Write(_horizontalBlankTransfer);
        }

        public void SetState(BinaryReader reader)
        {
            _sourceAddress = reader.ReadUInt16();
            _destAddress = reader.ReadUInt16();
            _transferLengthRemaining = reader.ReadByte();
            _transferActive = reader.ReadBoolean();
            _horizontalBlankTransferActive = reader.ReadBoolean();
            _horizontalBlankTransfer = reader.ReadBoolean();
        }
    }
}
