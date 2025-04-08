using System.IO;
using Atem.Core;
using Atem.Views.MonoGame.Audio;
using Atem.Views.MonoGame.Config;
using Atem.Views.MonoGame.Input;

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
            InputManager inputManager = new();
            _view = new View(_atem, configService, soundService, inputManager);
        }
    }
}
