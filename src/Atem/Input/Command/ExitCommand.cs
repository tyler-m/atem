using Atem.Views;

namespace Atem.Input.Command
{
    internal class ExitCommand : ICommand
    {
        private readonly IAtemView _view;

        public CommandType Type { get => CommandType.Exit; }

        public ExitCommand(IAtemView view)
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
