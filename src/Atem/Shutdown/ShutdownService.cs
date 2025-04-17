using Atem.Config;
using Atem.Core;
using Atem.Saving;

namespace Atem.Shutdown
{
    public class ShutdownService : IShutdownService
    {
        private readonly Emulator _emulator;
        private readonly IConfigService _configService;
        private readonly ICartridgeLoader _cartridgeLoader;
        private readonly IBatterySaveService _batterySaveService;

        public ShutdownService(Emulator emulator, IConfigService configService, ICartridgeLoader cartridgeLoader, IBatterySaveService batterySaveService)
        {
            _emulator = emulator;
            _configService = configService;
            _cartridgeLoader = cartridgeLoader;
            _batterySaveService = batterySaveService;
        }

        public void Shutdown()
        {
            if (_emulator.Cartridge.Loaded)
            {
                _batterySaveService.Save(_cartridgeLoader.Context);
            }

            _configService.SaveValues();
            _configService.SaveConfig();
        }
    }
}
