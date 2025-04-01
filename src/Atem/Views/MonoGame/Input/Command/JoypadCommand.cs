using Atem.Core.Input;

namespace Atem.Views.MonoGame.Input.Command
{
    internal class JoypadCommand : ICommand
    {
        private JoypadButton _button;

        public string Name => _button.ToString();

        public void Execute(View view, bool press)
        {
            view.Atem.OnJoypadChange(_button, press);
        }

        public JoypadCommand(JoypadButton button)
        {
            _button = button;
        }
    }
}
