using Atem.Saving;

namespace Atem.Input.Command
{
    public class LoadStateCommand : ICommand
    {
        private readonly ISaveStateService _saveStateService;
        private readonly ICartridgeContext _cartridgeContext;
        private readonly int _slot;

        public CommandType Type { get => CommandType.LoadState0 + _slot; }

        public LoadStateCommand(ISaveStateService saveStateService, ICartridgeContext cartridgeContext, int slot)
        {
            _saveStateService = saveStateService;
            _cartridgeContext = cartridgeContext;
            _slot = slot;
        }


        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _saveStateService.Load(_slot, _cartridgeContext);
            }
        }
    }
}
