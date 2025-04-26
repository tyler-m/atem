using System.IO;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core.Input
{
    public class SerialManager : IStateful
    {
        public object Lock = new();

        private byte _sb, _sc;
        private readonly Interrupt _interrupt;

        public byte SB { get => _sb; set => _sb = value; }

        public byte SC
        {
            get => _sc;
            set
            {
                _sc = value;

                OnTransferRequest?.Invoke();
            }
        }

        public bool TransferEnabled { get => _sc.GetBit(7); set => SC = _sc.SetBit(7, value); }
        public bool Master { get => _sc.GetBit(0); set => SC = _sc.SetBit(0, value); }

        public delegate void TransferRequestEvent();
        public event TransferRequestEvent OnTransferRequest;

        public delegate void ClockEvent();
        public event ClockEvent OnClock;

        public void Clock()
        {
            OnClock?.Invoke();
        }

        public SerialManager(Interrupt interrupt)
        {
            _interrupt = interrupt;
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

        public void RequestInterrupt()
        {
            _interrupt.SetInterrupt(InterruptType.Serial);
        }
    }
}
