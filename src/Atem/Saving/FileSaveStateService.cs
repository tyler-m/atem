using Atem.Core;
using System.IO;

namespace Atem.Saving
{
    public class FileSaveStateService : ISaveStateService
    {
        private AtemRunner _atem;

        public FileSaveStateService(AtemRunner atem)
        {
            _atem = atem;
        }

        public void Load(int slot, ICartridgeContext context)
        {
            string stateSavePath = context.Id + ".ss" + slot;

            if (File.Exists(stateSavePath))
            {
                byte[] saveStateData = File.ReadAllBytes(stateSavePath);
                _atem.SetState(saveStateData);
            }
        }

        public void Save(int slot, ICartridgeContext context)
        {
            File.WriteAllBytes(context.Id + ".ss" + slot, _atem.GetState());
        }
    }
}
