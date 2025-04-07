using Atem.Core;
using Atem.Views.Audio;
using Atem.Views.MonoGame.Audio;
using System.IO;

namespace Atem.Views.MonoGame
{
    internal class ViewStarter
    {
        private View _view;
        private AtemRunner _atem;
        private Config _config;

        public ViewStarter(AtemRunner atem)
        {
            _atem = atem;
        }

        public void Run()
        {
            string filePath = Directory.GetCurrentDirectory() + "/config.json";

            LoadConfig(filePath);
            CheckRomsDirectory();
            CreateView();

            _view.Run();
        }

        private void LoadConfig(string filePath)
        {
            // load config. if it doesn't exist, create it using defaults
            if (!File.Exists(filePath))
            {
                Config.CreateDefault(filePath);
            }

            _config = new Config(filePath);
        }

        private void CheckRomsDirectory()
        {   
            // ensure the roms directory exists. if it doesn't, create it
            if (!Directory.Exists(Path.GetFullPath(_config.RomsDirectory)))
            {
                Directory.CreateDirectory(Path.GetFullPath(_config.RomsDirectory));
            }
        }

        public void CreateView()
        {
            ISoundService soundService = new SoundService(_atem.Bus.Audio);
            _view = new View(_atem, _config, soundService);
        }
    }
}
