
using Atem.Core;

namespace Atem.Views.MonoGame.Input.Command
{
    public class PauseCommand : ICommand
    {
        private readonly AtemRunner _atem;

        public CommandType Type { get => CommandType.Pause; }

        public PauseCommand(AtemRunner atem)
        {
            _atem = atem;
        }

        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _atem.Paused = !_atem.Paused;
            }
        }
    }
}
