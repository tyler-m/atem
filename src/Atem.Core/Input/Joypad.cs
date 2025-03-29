using Atem.Core.Processing;
using Atem.Core.State;
using System.IO;

namespace Atem.Core.Input
{
    public enum JoypadButton
    {
        Up,
        Down,
        Left,
        Right,
        A,
        B,
        Select,
        Start
    }

    public class Joypad : IStateful
    {
        private byte _joypad;
        private byte _joyp;
        private readonly Bus _bus;

        private bool Up => _joypad.GetBit((int)JoypadButton.Up);
        private bool Down => _joypad.GetBit((int)JoypadButton.Down);
        private bool Left => _joypad.GetBit((int)JoypadButton.Left);
        private bool Right => _joypad.GetBit((int)JoypadButton.Right);
        private bool A => _joypad.GetBit((int)JoypadButton.A);
        private bool B => _joypad.GetBit((int)JoypadButton.B);
        private bool Select => _joypad.GetBit((int)JoypadButton.Select);
        private bool Start => _joypad.GetBit((int)JoypadButton.Start);

        public Joypad(Bus bus)
        {
            _bus = bus;
        }

        public byte P1
        {
            get
            {
                if (!_joyp.GetBit(5))
                {
                    return _joyp.SetBit(0, !A).SetBit(1, !B).SetBit(2, !Select).SetBit(3, !Start);
                }
                else if (!_joyp.GetBit(4))
                {
                    return _joyp.SetBit(0, !Right).SetBit(1, !Left).SetBit(2, !Up).SetBit(3, !Down);
                }
                else
                {
                    return 0xF;
                }
            }
            set
            {
                _joyp = _joyp.SetBit(5, value.GetBit(5)).SetBit(4, value.GetBit(4));
            }
        }

        public void OnJoypadChange(JoypadButton button, bool down)
        {
            _joypad = _joypad.SetBit((int)button, down);
            _bus.RequestInterrupt(InterruptType.Joypad);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_joypad);
            writer.Write(_joyp);
        }

        public void SetState(BinaryReader reader)
        {
            _joypad = reader.ReadByte();
            _joyp = reader.ReadByte();
        }
    }
}
