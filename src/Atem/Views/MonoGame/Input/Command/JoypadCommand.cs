using Atem.Core.Input;

namespace Atem.Views.MonoGame.Input.Command
{
    internal class JoypadCommand : ICommand
    {
        private View _view;
        private JoypadButton _joypadButton;
        private CommandType _commandType;

        public CommandType Type { get { return _commandType; } }

        public JoypadCommand(View view, JoypadButton joypadButton, CommandType commandType)
        {
            _view = view;
            _joypadButton = joypadButton;
            _commandType = commandType;
        }

        public void Execute(bool pressed)
        {
            _view.Atem.OnJoypadChange(_joypadButton, pressed);
        }
    }
}
