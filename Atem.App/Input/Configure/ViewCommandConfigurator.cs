using Atem.App.Input.Command;
using Atem.App.View;

namespace Atem.App.Input.Configure
{
    public class ViewCommandConfigurator
    {
        private readonly IAtemView _view;

        public ViewCommandConfigurator(IAtemView view)
        {
            _view = view;
        }

        public void Configure(InputManager inputManager)
        {
            inputManager.AddCommand(new ExitCommand(_view));
        }
    }
}