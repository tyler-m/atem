using System;
using System.Collections.Generic;
using System.IO;
using Atem.Core.Processing;

namespace Atem.Core.Input
{
    public class SerialManager : ISerialManager
    {
        private readonly Interrupt _interrupt;
        private byte _sc;

        public event EventHandler<EventArgs> OnTransferRequest;
        public event EventHandler<EventArgs> OnClock;

        public byte SC
        {
            get => _sc;
            set
            {
                _sc = value;

                OnTransferRequest?.Invoke(this, EventArgs.Empty);
            }
        }

        public byte SB { get; set; }
        public bool TransferEnabled { get => _sc.GetBit(7); set => SC = _sc.SetBit(7, value); }
        public bool Master { get => _sc.GetBit(0); set => SC = _sc.SetBit(0, value); }

        public SerialManager(Interrupt interrupt)
        {
            _interrupt = interrupt;
        }

        public void Clock()
        {
            OnClock?.Invoke(this, EventArgs.Empty);
        }

        public void RequestInterrupt()
        {
            _interrupt.SetInterrupt(InterruptType.Serial);
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            return address switch
            {
                0xFF01 => SB,
                0xFF02 => SC,
                _ => 0xFF
            };
        }

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false)
        {
            switch (address)
            {
                case 0xFF01:
                    SB = value;
                    break;
                case 0xFF02:
                    SC = value;
                    break;
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetMemoryRanges()
        {
            yield return (0xFF01, 0xFF02); // registers
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(SB);
            writer.Write(SC);
        }

        public void SetState(BinaryReader reader)
        {
            SB = reader.ReadByte();
            SC = reader.ReadByte();
        }
    }
}
