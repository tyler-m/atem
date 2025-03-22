using Atem.Core;
using System;
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
            // load config. if it doesn't exist, create it using defaults
            try
            {
                _config = Config.Load(Directory.GetCurrentDirectory() + "/config.json");
            }
            catch (FileNotFoundException e)
            {
                Config.CreateDefault(Directory.GetCurrentDirectory() + "/config.json");

                try
                {
                    _config = Config.Load(Directory.GetCurrentDirectory() + "/config.json");
                }
                catch
                {
                    throw new Exception("Unable to create default config file.");
                }
            }

            // ensure the roms directory exists. if it doesn't, create it
            if (!Directory.Exists(Path.GetFullPath(_config.RomsDirectory)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetFullPath(_config.RomsDirectory));
                }
                catch
                {
                    throw new Exception("Unable to locate or create roms directory.");
                }
            }

            _view = new View(_atem, _config);
            _view.Run();
        }
    }
}
