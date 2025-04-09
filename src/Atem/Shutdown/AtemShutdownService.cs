using Atem.Config;
using Atem.Core;
using Atem.Saving;

namespace Atem.Shutdown
{
    public class AtemShutdownService : IShutdownService
    {
        private readonly AtemRunner _atem;
        private readonly IConfigService _configService;
        private readonly ICartridgeLoader _cartridgeLoader;
        private readonly IBatterySaveService _batterySaveService;

        public AtemShutdownService(AtemRunner atem, IConfigService configService, ICartridgeLoader cartridgeLoader, IBatterySaveService batterySaveService)
        {
            _atem = atem;
            _configService = configService;
            _cartridgeLoader = cartridgeLoader;
            _batterySaveService = batterySaveService;
        }

        public void Shutdown()
        {
            if (_atem.Bus.Cartridge.Loaded)
            {
                _batterySaveService.Save(_cartridgeLoader.Context);
            }

            _configService.Save();
        }
    }
}
