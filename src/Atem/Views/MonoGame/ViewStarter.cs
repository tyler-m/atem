using System.IO;
using Atem.Core;
using Atem.Views.MonoGame.Audio;
using Atem.Views.MonoGame.Config;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.Saving;

namespace Atem.Views.MonoGame
{
    public class ViewStarter
    {
        private View _view;
        private readonly AtemRunner _atem;

        public ViewStarter(AtemRunner atem)
        {
            _atem = atem;
        }

        public void Run()
        {
            InitializeView();

            _view.Run();
        }

        public void InitializeView()
        {
            ViewConfigFileStore configStore = new(Directory.GetCurrentDirectory() + "/config.json");
            ViewConfigService configService = new(configStore);
            SoundService soundService = new(_atem.Bus.Audio);
            FileSaveStateService saveStateService = new(_atem);
            FileCartridgeLoader cartridgeLoader = new(_atem);
            FileBatterySaveService batterySaveService = new(_atem);
            InputManager inputManager = new();
            
            _view = new View(_atem, configService, soundService, saveStateService, cartridgeLoader, batterySaveService, inputManager);

            ViewCommandConfigurator viewCommandConfigurator = new(_view);
            viewCommandConfigurator.Configure(inputManager);
        }
    }
}
