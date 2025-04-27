using System.IO;
using Atem.Core.State;

namespace Atem.Core.Graphics.Tiles
{
    public class TileMap : IStateful
    {
        private readonly byte[] _map = new byte[32 * 32 * 2];
        private readonly TileAttribute[] _attributes = new TileAttribute[32 * 32 * 2];

        public TileMap()
        {
            for (int i = 0; i < _attributes.Length; i++)
            {
                _attributes[i] = new TileAttribute();
            }
        }

        public void SetTileIndex(int offset, byte value)
        {
            _map[offset] = value;
        }

        public byte GetTileIndex(int offset)
        {
            return _map[offset];
        }

        public TileAttribute GetTileAttribute(int offset)
        {
            return _attributes[offset];
        }

        public byte GetTileIndexOfTileMapArea(int tileMapX, int tileMapY, int area)
        {
            int offset = tileMapX + (tileMapY * 32) + (area * 32 * 32);
            return GetTileIndex(offset);
        }

        public TileAttribute GetTileAttributeOfTileMapArea(int tileMapX, int tileMapY, int area)
        {
            int offset = tileMapX + (tileMapY * 32) + (area * 32 * 32);
            return GetTileAttribute(offset);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_map);
            
            foreach (TileAttribute tileAttribute in _attributes)
            {
                tileAttribute.GetState(writer);
            }
        }

        public void SetState(BinaryReader reader)
        {
            reader.ReadBytes(_map.Length).CopyTo(_map, 0);

            foreach (TileAttribute tileAttribute in _attributes)
            {
                tileAttribute.SetState(reader);
            }
        }
    }
}