using System;
using System.IO;
using Atem.Core;

namespace Atem.Saving
{
    public class FileCartridgeLoader : ICartridgeLoader
    {
        private readonly AtemRunner _atem;
        private readonly FileCartridgeContext _context;

        public ICartridgeContext Context { get => _context; }

        public FileCartridgeLoader(AtemRunner atem)
        {
            _atem = atem;
            _context = new FileCartridgeContext();
        }

        public bool Load()
        {
            string filePath = _context.Id;

            if (!File.Exists(filePath))
            {
                throw new Exception();
            }

            byte[] data = File.ReadAllBytes(filePath);

            return _atem.LoadCartridge(data, filePath.ToLower().EndsWith(".gbc"));
        }
    }
}
