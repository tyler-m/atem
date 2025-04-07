using System.IO;
using Atem.Core;
using Atem.Views.MonoGame.Audio;
using Atem.Views.MonoGame.Config;

namespace Atem.Views.MonoGame
{
    public class ViewStarter
    {
        private View _view;
        private readonly AtemRunner _atem;
        private ViewConfig _config;

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
            _config = new ViewConfig();
            ViewConfigFileStore configStore = new(Directory.GetCurrentDirectory() + "/config.json");
            ViewConfigService configService = new(_config, configStore);

            SoundService soundService = new(_atem.Bus.Audio);

            _view = new View(_atem, configService, soundService);

            CheckRomsDirectory();
        }

        private void CheckRomsDirectory()
        {
            // ensure the roms directory exists. if it doesn't, create it
            if (!Directory.Exists(Path.GetFullPath(_config.RomsDirectory)))
            {
                Directory.CreateDirectory(Path.GetFullPath(_config.RomsDirectory));
            }
        }
    }
}
