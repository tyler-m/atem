using System.IO;
using Atem.Core;

namespace Atem.Saving
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
                _atem.Cartridge.LoadBatterySave(saveData);
            }
        }

        public void Save(ICartridgeContext context)
        {
            File.WriteAllBytes(context.Id + ".sav", _atem.Cartridge.GetBatterySave());
        }
    }
}
