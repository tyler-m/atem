using System.IO;
using Atem.Config;
using Atem.Core;
using Atem.Graphics;
using Atem.Input;
using Atem.Input.Configure;
using Atem.IO;
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
        private readonly Emulator _emulator;

        public ViewStarter(Emulator emulator)
        {
            _emulator = emulator;
        }

        public void Run()
        {
            InitializeView();

            _view.Run();
        }

        public void InitializeView()
        {
            SoundService soundService = new(_emulator.Audio);
            FileSaveStateService saveStateService = new(_emulator);
            FileCartridgeLoader cartridgeLoader = new(_emulator);
            FileBatterySaveService batterySaveService = new(_emulator);
            InputManager inputManager = new(new KeyProvider());
            ImGuiRenderer imGui = new();
            Window window = new();
            Screen screen = new(_emulator, window);
            RecentFilesService recentFilesService = new();

            AtemConfigDefaultsProvider atemConfigDefaultsProvider = new();
            FileConfigStore<AtemConfig> configStore = new(atemConfigDefaultsProvider, Directory.GetCurrentDirectory() + "/config.json");
            AtemConfigService configService = new(configStore, window, screen, _emulator.Audio, inputManager, recentFilesService);
            ViewUIManager viewUIManager = new(imGui, _emulator, saveStateService, batterySaveService, cartridgeLoader, screen, inputManager, recentFilesService);
            AtemShutdownService shutdownService = new(_emulator, configService, cartridgeLoader, batterySaveService);

            _view = new View(viewUIManager, _emulator, screen, window, soundService, inputManager, shutdownService);
            _view.OnInitialize += () => imGui.Initialize(_view); // link ImGuiRenderer and View instances

            // add commands to the input manager
            AtemCommandConfigurator atemCommandConfigurator = new(_emulator);
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
