
namespace Atem.Views.MonoGame.Input.Command
{
    public class PauseCommand : ICommand
    {
        private readonly View _view;

        public CommandType Type => CommandType.Pause;

        public PauseCommand(View view)
        {
            _view = view;
        }

        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _view.Atem.Paused = !_view.Atem.Paused;
            }
        }
    }
}
