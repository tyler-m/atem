using Atem.Core;

namespace Atem.Input.Command
{
    public class PauseCommand : ICommand
    {
        private readonly Emulator _emulator;

        public CommandType Type { get => CommandType.Pause; }

        public PauseCommand(Emulator emulator)
        {
            _emulator = emulator;
        }

        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _emulator.Paused = !_emulator.Paused;
            }
        }
    }
}
