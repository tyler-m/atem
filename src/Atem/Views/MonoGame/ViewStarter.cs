using System.IO;
using Atem.Core;
using Atem.Views.MonoGame.Audio;
using Atem.Views.MonoGame.Config;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.Saving;
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
            ViewConfigFileStore configStore = new(Directory.GetCurrentDirectory() + "/config.json");
            SoundService soundService = new(_atem.Bus.Audio);
            FileSaveStateService saveStateService = new(_atem);
            FileCartridgeLoader cartridgeLoader = new(_atem);
            FileBatterySaveService batterySaveService = new(_atem);
            InputManager inputManager = new();
            ImGuiRenderer imGui = new();
            Screen screen = new(_atem);
            ViewConfigService configService = new(configStore, screen, _atem.Bus.Audio, inputManager);
            ViewUIManager viewUIManager = new(imGui, _atem, saveStateService, batterySaveService, cartridgeLoader, screen, inputManager);
            AtemShutdownService shutdownService = new(_atem, configService, cartridgeLoader, batterySaveService);

            _view = new View(viewUIManager, _atem, screen, soundService, inputManager, shutdownService);
            _view.OnInitialize += () => imGui.Initialize(_view); // link ImGuiRenderer and View instances

            // add view related commands to the input manager
            ViewCommandConfigurator viewCommandConfigurator = new(_view);
            viewCommandConfigurator.Configure(inputManager);

            configService.Load();
        }
    }
}
