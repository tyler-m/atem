using Atem.Core;

namespace Atem.Views.MonoGame.Input.Command
{
    public class ContinueCommand : ICommand
    {
        private readonly AtemRunner _atem;

        public CommandType Type { get => CommandType.Continue; }

        public ContinueCommand(AtemRunner atem)
        {
            _atem = atem;
        }

        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _atem.Continue();
            }
        }
    }
}
