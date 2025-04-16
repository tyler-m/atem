using Atem.Core;
using Atem.Core.Input;

namespace Atem.Input.Command
{
    internal class JoypadCommand : ICommand
    {
        private readonly Emulator _emulator;
        private readonly JoypadButton _joypadButton;
        private readonly CommandType _commandType;

        public CommandType Type { get => _commandType; }

        public JoypadCommand(Emulator emulator, JoypadButton joypadButton, CommandType commandType)
        {
            _emulator = emulator;
            _joypadButton = joypadButton;
            _commandType = commandType;
        }

        public void Execute(bool pressed)
        {
            _emulator.OnJoypadChange(_joypadButton, pressed);
        }
    }
}
