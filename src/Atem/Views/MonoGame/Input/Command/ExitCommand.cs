
namespace Atem.Views.MonoGame.Input.Command
{
    internal class ExitCommand : ICommand
    {
        private readonly View _view;

        public CommandType Type => CommandType.Exit;

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
