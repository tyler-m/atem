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
            string saveStatePath = context.Id + ".ss" + slot;

            if (File.Exists(saveStatePath))
            {
                using FileStream stream = new(saveStatePath, FileMode.Open);
                using BinaryReader reader = new(stream);
                _atem.SetState(reader);
            }
        }

        public void Save(int slot, ICartridgeContext context)
        {
            string saveStatePath = context.Id + ".ss" + slot;

            using FileStream stream = new(saveStatePath, FileMode.Create);
            using BinaryWriter writer = new(stream);
            _atem.GetState(writer);
        }
    }
}
