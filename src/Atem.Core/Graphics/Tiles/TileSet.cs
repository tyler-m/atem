using System.IO;
using Atem.Core.State;

namespace Atem.Core.Graphics.Tiles
{
    public class TileSet : IStateful
    {
        public const int BankSize = 384;
        public const int BankCount = 2;

        private readonly Tile[] _tiles = new Tile[BankCount * BankSize];

        public TileSet()
        {
            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = new Tile();
            }
        }

        public Tile GetTile(int index, byte bank)
        {
            return _tiles[index + (bank * BankSize)];
        }

        public void GetState(BinaryWriter writer)
        {
            foreach (Tile tile in _tiles)
            {
                tile.GetState(writer);
            }
        }

        public void SetState(BinaryReader reader)
        {
            foreach (Tile tile in _tiles)
            {
                tile.SetState(reader);
            }
        }
    }
}