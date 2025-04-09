using Atem.Core;
using Atem.Core.Input;

namespace Atem.Input.Command
{
    public class AtemCommandConfigurator
    {
        private readonly AtemRunner _atem;

        public AtemCommandConfigurator(AtemRunner atem)
        {
            _atem = atem;
        }

        public void Configure(InputManager inputManager)
        {
            inputManager.AddCommand(new PauseCommand(_atem));
            inputManager.AddCommand(new ContinueCommand(_atem));
            inputManager.AddCommand(new JoypadCommand(_atem, JoypadButton.Up, CommandType.Up));
            inputManager.AddCommand(new JoypadCommand(_atem, JoypadButton.Down, CommandType.Down));
            inputManager.AddCommand(new JoypadCommand(_atem, JoypadButton.Left, CommandType.Left));
            inputManager.AddCommand(new JoypadCommand(_atem, JoypadButton.Right, CommandType.Right));
            inputManager.AddCommand(new JoypadCommand(_atem, JoypadButton.B, CommandType.B));
            inputManager.AddCommand(new JoypadCommand(_atem, JoypadButton.A, CommandType.A));
            inputManager.AddCommand(new JoypadCommand(_atem, JoypadButton.Select, CommandType.Select));
            inputManager.AddCommand(new JoypadCommand(_atem, JoypadButton.Start, CommandType.Start));
        }
    }
}