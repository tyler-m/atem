using Atem.Input;
using Atem.Views.MonoGame.Input.Command;

namespace Atem.Views.MonoGame.Input
{
    public class ViewCommandConfigurator
    {
        private readonly View _view;

        public ViewCommandConfigurator(View view)
        {
            _view = view;
        }

        public void Configure(InputManager inputManager)
        {
            inputManager.AddCommand(new ExitCommand(_view));
        }
    }
}