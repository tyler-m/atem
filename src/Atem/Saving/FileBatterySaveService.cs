using System.IO;
using Atem.Core;

namespace Atem.Saving
{
    public class FileBatterySaveService : IBatterySaveService
    {
        private readonly IEmulator _emulator;

        public FileBatterySaveService(IEmulator emulator)
        {
            _emulator = emulator;
        }

        public void Load(ICartridgeContext context)
        {
            string savePath = context.Id + ".sav";

            if (File.Exists(savePath))
            {
                byte[] saveData = File.ReadAllBytes(savePath);
                _emulator.Cartridge.LoadBatterySave(saveData);
            }
        }

        public void Save(ICartridgeContext context)
        {
            File.WriteAllBytes(context.Id + ".sav", _emulator.Cartridge.GetBatterySave());
        }
    }
}
