using Atem.Core.Input;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.Input.Command;

namespace Atem.Views.MonoGame
{
    public class ViewCommandConfigurator
    {
        private readonly View _view;

        public ViewCommandConfigurator(View view)
        {
            _view = view;
        }

        public void Configure(InputManager inputManager)
        {
            inputManager.AddCommand(new ExitCommand(_view));
            inputManager.AddCommand(new ContinueCommand(_view));
            inputManager.AddCommand(new PauseCommand(_view));
            inputManager.AddCommand(new JoypadCommand(_view, JoypadButton.Up, CommandType.Up));
            inputManager.AddCommand(new JoypadCommand(_view, JoypadButton.Down, CommandType.Down));
            inputManager.AddCommand(new JoypadCommand(_view, JoypadButton.Left, CommandType.Left));
            inputManager.AddCommand(new JoypadCommand(_view, JoypadButton.Right, CommandType.Right));
            inputManager.AddCommand(new JoypadCommand(_view, JoypadButton.B, CommandType.B));
            inputManager.AddCommand(new JoypadCommand(_view, JoypadButton.A, CommandType.A));
            inputManager.AddCommand(new JoypadCommand(_view, JoypadButton.Select, CommandType.Select));
            inputManager.AddCommand(new JoypadCommand(_view, JoypadButton.Start, CommandType.Start));
        }
    }
}