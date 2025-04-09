using Atem.Core;
using Atem.Core.Input;

namespace Atem.Input.Command
{
    internal class JoypadCommand : ICommand
    {
        private readonly AtemRunner _atem;
        private readonly JoypadButton _joypadButton;
        private readonly CommandType _commandType;

        public CommandType Type { get => _commandType; }

        public JoypadCommand(AtemRunner atem, JoypadButton joypadButton, CommandType commandType)
        {
            _atem = atem;
            _joypadButton = joypadButton;
            _commandType = commandType;
        }

        public void Execute(bool pressed)
        {
            _atem.OnJoypadChange(_joypadButton, pressed);
        }
    }
}
