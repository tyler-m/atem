
namespace Atem.Views.MonoGame.Input.Command
{
    public class ContinueCommand : ICommand
    {
        private readonly View _view;

        public CommandType Type => CommandType.Continue;

        public ContinueCommand(View view)
        {
            _view = view;
        }

        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _view.Atem.Continue();
            }
        }
    }
}
