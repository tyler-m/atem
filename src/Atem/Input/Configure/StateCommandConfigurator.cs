using Atem.Input.Command;
using Atem.Saving;

namespace Atem.Input.Configure
{
    internal class StateCommandConfigurator
    {
        private readonly ISaveStateService _saveStateService;
        private readonly ICartridgeContext _cartridgeContext;

        public StateCommandConfigurator(ISaveStateService saveStateService, ICartridgeContext cartridgeContext)
        {
            _saveStateService = saveStateService;
            _cartridgeContext = cartridgeContext;
        }

        public void Configure(InputManager inputManager)
        {
            for (int i = 0; i < 10; i++)
            {
                inputManager.AddCommand(new LoadStateCommand(_saveStateService, _cartridgeContext, i));
            }

            for (int i = 0; i < 10; i++)
            {
                inputManager.AddCommand(new SaveStateCommand(_saveStateService, _cartridgeContext, i));
            }
        }
    }
}
