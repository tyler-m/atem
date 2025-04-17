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
    public static class ViewStarter
    {
        public static View Start(Emulator emulator)
        {
            Window window = new();
            Screen screen = new(emulator, window);
            SoundService soundService = new(emulator.Audio);
            FileSaveStateService saveStateService = new(emulator);
            FileCartridgeLoader cartridgeLoader = new(emulator);
            FileBatterySaveService batterySaveService = new(emulator);
            InputManager inputManager = new(new KeyProvider());
            RecentFilesService recentFilesService = new();

            AtemConfigDefaultsProvider atemConfigDefaultsProvider = new();
            FileConfigStore<AtemConfig> configStore = new(atemConfigDefaultsProvider, Directory.GetCurrentDirectory() + "/config.json");
            AtemConfigService configService = new(configStore, window, screen, emulator.Audio, inputManager, recentFilesService);
            ShutdownService shutdownService = new(emulator, configService, cartridgeLoader, batterySaveService);

            ImGuiRenderer imGui = new();
            ViewUIManager viewUIManager = new(imGui, emulator, saveStateService, batterySaveService, cartridgeLoader, screen, inputManager, recentFilesService);

            View view = new View(viewUIManager, emulator, screen, window, soundService, inputManager, shutdownService);
            view.OnInitialize += () => imGui.Initialize(view); // link ImGuiRenderer and View instances

            // add commands to the input manager
            AtemCommandConfigurator atemCommandConfigurator = new(emulator);
            ViewCommandConfigurator viewCommandConfigurator = new(view);
            StateCommandConfigurator stateCommandConfigurator = new(saveStateService, cartridgeLoader.Context);
            atemCommandConfigurator.Configure(inputManager);
            viewCommandConfigurator.Configure(inputManager);
            stateCommandConfigurator.Configure(inputManager);

            configService.LoadConfig(); // grab config from file
            configService.LoadValues(); // load values from config

            view.Run();
            return view;
        }
    }
}
