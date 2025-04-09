using Atem.Input.Command;
using Atem.Views;

namespace Atem.Input.Configure
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