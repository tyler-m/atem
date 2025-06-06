﻿using System.Collections.Generic;
using System.IO;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core.State;

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

    public class Joypad : IAddressable, IBootable, IStateful
    {
        private byte _joypad;
        private byte _joyp;
        private readonly Interrupt _interrupt;

        private bool Up => _joypad.GetBit((int)JoypadButton.Up);
        private bool Down => _joypad.GetBit((int)JoypadButton.Down);
        private bool Left => _joypad.GetBit((int)JoypadButton.Left);
        private bool Right => _joypad.GetBit((int)JoypadButton.Right);
        private bool A => _joypad.GetBit((int)JoypadButton.A);
        private bool B => _joypad.GetBit((int)JoypadButton.B);
        private bool Select => _joypad.GetBit((int)JoypadButton.Select);
        private bool Start => _joypad.GetBit((int)JoypadButton.Start);

        public Joypad(Interrupt interrupt)
        {
            _interrupt = interrupt;
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
            _interrupt.SetInterrupt(InterruptType.Joypad);
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            return address switch
            {
                0xFF00 => P1,
                _ => 0xFF
            };
        }

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false)
        {
            switch (address)
            {
                case 0xFF00:
                    P1 = value;
                    break;
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetAddressRanges()
        {
            yield return (0xFF00, 0xFF00); // P1 register
        }

        public void Boot(BootMode mode)
        {
            switch (mode)
            {
                case BootMode.CGB:
                    P1 = 0xC7;
                    break;
                case BootMode.DMG:
                    P1 = 0xCF;
                    break;
            }
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
