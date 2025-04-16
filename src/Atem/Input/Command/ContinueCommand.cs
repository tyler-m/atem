using Atem.Core;

namespace Atem.Input.Command
{
    public class ContinueCommand : ICommand
    {
        private readonly Emulator _emulator;

        public CommandType Type { get => CommandType.Continue; }

        public ContinueCommand(Emulator emulator)
        {
            _emulator = emulator;
        }

        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _emulator.Continue();
            }
        }
    }
}
