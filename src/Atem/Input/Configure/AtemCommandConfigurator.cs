using Atem.Core;
using Atem.Core.Input;
using Atem.Input.Command;

namespace Atem.Input.Configure
{
    public class AtemCommandConfigurator
    {
        private readonly Emulator _emulator;

        public AtemCommandConfigurator(Emulator emulator)
        {
            _emulator = emulator;
        }

        public void Configure(InputManager inputManager)
        {
            inputManager.AddCommand(new PauseCommand(_emulator));
            inputManager.AddCommand(new ContinueCommand(_emulator));
            inputManager.AddCommand(new JoypadCommand(_emulator, JoypadButton.Up, CommandType.Up));
            inputManager.AddCommand(new JoypadCommand(_emulator, JoypadButton.Down, CommandType.Down));
            inputManager.AddCommand(new JoypadCommand(_emulator, JoypadButton.Left, CommandType.Left));
            inputManager.AddCommand(new JoypadCommand(_emulator, JoypadButton.Right, CommandType.Right));
            inputManager.AddCommand(new JoypadCommand(_emulator, JoypadButton.B, CommandType.B));
            inputManager.AddCommand(new JoypadCommand(_emulator, JoypadButton.A, CommandType.A));
            inputManager.AddCommand(new JoypadCommand(_emulator, JoypadButton.Select, CommandType.Select));
            inputManager.AddCommand(new JoypadCommand(_emulator, JoypadButton.Start, CommandType.Start));
        }
    }
}