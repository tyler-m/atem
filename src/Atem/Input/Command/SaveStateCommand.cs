using Atem.Saving;

namespace Atem.Input.Command
{
    public class SaveStateCommand : ICommand
    {
        private readonly ISaveStateService _saveStateService;
        private readonly ICartridgeContext _cartridgeContext;
        private readonly int _slot;

        public CommandType Type { get => CommandType.SaveState0 + _slot; }

        public SaveStateCommand(ISaveStateService saveStateService, ICartridgeContext cartridgeContext, int slot)
        {
            _saveStateService = saveStateService;
            _cartridgeContext = cartridgeContext;
            _slot = slot;
        }


        public void Execute(bool pressed)
        {
            if (pressed)
            {
                _saveStateService.Save(_slot, _cartridgeContext);
            }
        }
    }
}
