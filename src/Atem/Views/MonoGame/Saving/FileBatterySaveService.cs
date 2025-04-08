using System.IO;
using Atem.Core;

namespace Atem.Views.MonoGame.Saving
{
    public class FileBatterySaveService : IBatterySaveService
    {
        private readonly AtemRunner _atem;

        public FileBatterySaveService(AtemRunner atem)
        {
            _atem = atem;
        }

        public void Load(ICartridgeContext context)
        {
            string savePath = context.Id + ".sav";

            if (File.Exists(savePath))
            {
                byte[] saveData = File.ReadAllBytes(savePath);
                _atem.Bus.Cartridge.LoadBatterySave(saveData);
            }
        }

        public void Save(ICartridgeContext context)
        {
            File.WriteAllBytes(context.Id + ".sav", _atem.Bus.Cartridge.GetBatterySave());
        }
    }
}
