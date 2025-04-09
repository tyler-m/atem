
using Atem.Input.Command;

namespace Atem.Views.MonoGame.Input.Command
{
    internal class ExitCommand : ICommand
    {
        private readonly View _view;

        public CommandType Type { get => CommandType.Exit; }

        public ExitCommand(View view)
        {
            _view = view;
        }

        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _view.Exit();
            }
        }
    }
}
