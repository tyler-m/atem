using System.IO;
using Atem.Config;
using Atem.Core;
using Atem.Graphics;
using Atem.Input;
using Atem.Input.Configure;
using Atem.Saving;
using Atem.Shutdown;
using Atem.Views.MonoGame.Audio;
using Atem.Views.MonoGame.Config;
using Atem.Views.MonoGame.Graphics;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.UI;

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
            SoundService soundService = new(_atem.Bus.Audio);
            FileSaveStateService saveStateService = new(_atem);
            FileCartridgeLoader cartridgeLoader = new(_atem);
            FileBatterySaveService batterySaveService = new(_atem);
            InputManager inputManager = new(new KeyProvider());
            ImGuiRenderer imGui = new();
            Window window = new();
            Screen screen = new(_atem, window);

            AtemConfigDefaultsProvider atemConfigDefaultsProvider = new();
            FileConfigStore<AtemConfig> configStore = new(atemConfigDefaultsProvider, Directory.GetCurrentDirectory() + "/config.json");
            AtemConfigService configService = new(configStore, window, screen, _atem.Bus.Audio, inputManager);
            ViewUIManager viewUIManager = new(imGui, _atem, saveStateService, batterySaveService, cartridgeLoader, screen, inputManager);
            AtemShutdownService shutdownService = new(_atem, configService, cartridgeLoader, batterySaveService);

            _view = new View(viewUIManager, _atem, screen, window, soundService, inputManager, shutdownService);
            _view.OnInitialize += () => imGui.Initialize(_view); // link ImGuiRenderer and View instances

            // add commands to the input manager
            AtemCommandConfigurator atemCommandConfigurator = new(_atem);
            ViewCommandConfigurator viewCommandConfigurator = new(_view);
            StateCommandConfigurator stateCommandConfigurator = new(saveStateService, cartridgeLoader.Context);
            atemCommandConfigurator.Configure(inputManager);
            viewCommandConfigurator.Configure(inputManager);
            stateCommandConfigurator.Configure(inputManager);

            configService.LoadConfig(); // grab config from file
            configService.LoadValues(); // load values from config
        }
    }
}
