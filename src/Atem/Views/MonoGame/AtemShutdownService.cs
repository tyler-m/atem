﻿using Atem.Core;
using Atem.Views.MonoGame.Config;
using Atem.Views.MonoGame.Saving;

namespace Atem.Views.MonoGame
{
    public class AtemShutdownService : IShutdownService
    {
        private readonly AtemRunner _atem;
        private readonly IViewConfigService _configService;
        private readonly ICartridgeLoader _cartridgeLoader;
        private readonly IBatterySaveService _batterySaveService;

        public AtemShutdownService(AtemRunner atem, IViewConfigService configService, ICartridgeLoader cartridgeLoader, IBatterySaveService batterySaveService)
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
