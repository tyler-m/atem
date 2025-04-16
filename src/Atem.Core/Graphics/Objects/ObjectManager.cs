using System.Collections.Generic;
using System.IO;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;
using Atem.Core.Memory;

namespace Atem.Core.Graphics.Objects
{
    public class ObjectManager : IObjectManager
    {
        private const int MAX_SPRITES = 40;
        private const int SPRITE_BUFFER_LIMIT = 10;

        private readonly IBus _bus;
        private readonly IRenderModeScheduler _renderModeScheduler;
        private readonly ITileManager _tileManager;
        private readonly IPaletteProvider _paletteProvider;
        private readonly Cartridge _cartridge;
        private readonly Sprite[] _objects = new Sprite[MAX_SPRITES];
        private readonly List<Sprite> _spriteBuffer = [];
        private bool _objectsEnabled;
        private int _objectIndex;
        private bool _largeObjects;
        private byte _odma;

        public bool ObjectsEnabled { get => _objectsEnabled; set => _objectsEnabled = value; }
        public bool LargeObjects { get => _largeObjects; set => _largeObjects = value; }

        public byte ODMA
        {
            get
            {
                return _odma;
            }
            set
            {
                TriggerODMA(value);
                _odma = value;
            }
        }

        public ObjectManager(IBus bus, IRenderModeScheduler renderModeScheduler, ITileManager tileManager, IPaletteProvider paletteProvider, Cartridge cartridge)
        {
            _bus = bus;
            _renderModeScheduler = renderModeScheduler;
            _tileManager = tileManager;
            _paletteProvider = paletteProvider;
            _cartridge = cartridge;

            for (int i = 0; i < MAX_SPRITES; i++)
            {
                _objects[i] = new Sprite();
            }

            _renderModeScheduler.RenderModeChanged += RenderModeChanged;
        }

        private void RenderModeChanged(object sender, RenderModeChangedEventArgs e)
        {
            if (e.CurrentMode == RenderMode.OAM)
            {
                _spriteBuffer.Clear();
                _objectIndex = 0;
            }
        }

        private void TriggerODMA(byte value)
        {
            int address = value << 8;

            for (int i = 0; i < MAX_SPRITES; i++)
            {
                _objects[i].Populate(
                    _bus.Read((ushort)(address + 4 * i)),
                    _bus.Read((ushort)(address + 4 * i + 1)),
                    _bus.Read((ushort)(address + 4 * i + 2)),
                    _bus.Read((ushort)(address + 4 * i + 3)));
            }
        }

        public void CollectObjectsForScanline()
        {
            // process 2 sprites per tick to spread OAM search across scanline,
            // emulating real hardware behavior
            for (int i = 0; i < 2; i++)
            {
                // find object to add to the object buffer for the current line
                if (_spriteBuffer.Count < SPRITE_BUFFER_LIMIT)
                {
                    Sprite sprite = _objects[_objectIndex++];

                    int spriteHeight = _largeObjects ? 16 : 8;
                    if (sprite.X > 0 && _renderModeScheduler.CurrentLine + 16 >= sprite.Y && _renderModeScheduler.CurrentLine + 16 < sprite.Y + spriteHeight)
                    {
                        _spriteBuffer.Add(sprite);
                    }
                }
            }
        }

        private int GetSpriteId(Sprite sprite, int pixelX, int pixelY)
        {
            int offsetX = pixelX - (sprite.X - 8); // coordinates of pixel inside tile at (x, y)
            int offsetY = pixelY - (sprite.Y - 16);
            byte spriteTile = sprite.Tile;

            if (_largeObjects)
            {
                // ignore first bit of tile with 8x16 objects
                spriteTile &= 0b11111110;

                if (sprite.FlipY)
                {
                    offsetY = 15 - offsetY;
                }
            }
            else
            {
                if (sprite.FlipY)
                {
                    offsetY = 7 - offsetY;
                }
            }

            int tileDataAddress = spriteTile * 16 + (_cartridge.SupportsColor ? 0x2000 * sprite.Bank : 0);
            int id = _tileManager.GetTileId(tileDataAddress, offsetX, offsetY, sprite.FlipX);
            return id;
        }

        public (GBColor color, int spriteId, Sprite sprite) GetSpritePixelInfo(int x, int y)
        {
            GBColor spriteColor = null;
            Sprite sprite = null;
            int spriteId = 0;

            for (int j = 0; j < _spriteBuffer.Count; j++)
            {
                Sprite tempSprite = _spriteBuffer[j];

                if (tempSprite.X > x && tempSprite.X <= x + 8)
                {
                    int id = GetSpriteId(tempSprite, x, y);

                    if (id == 0)
                    {
                        continue;
                    }

                    if (_cartridge.SupportsColor)
                    {
                        // in color mode object locations in OAM determines priority
                        // (the object list is populated in order of OAM location, so we
                        // just take the first visible object we encounter in the list)
                        if (sprite == null)
                        {
                            spriteColor = _paletteProvider.ObjectPalettes[tempSprite.ColorPalette][id];
                            sprite = tempSprite;
                            spriteId = id;
                        }
                    }
                    else
                    {
                        // in no-color mode, object X coordinates determine priority.
                        // if X coordinates between objects are identical, the object
                        // earliest in OAM is prioritized
                        if (sprite == null || tempSprite.X < sprite.X)
                        {
                            if (tempSprite.Palette)
                            {
                                spriteColor = _paletteProvider.DMGPalettes[2][id];
                            }
                            else
                            {
                                spriteColor = _paletteProvider.DMGPalettes[1][id];
                            }

                            sprite = tempSprite;
                            spriteId = id;
                        }
                    }
                }
            }

            return (spriteColor, spriteId, sprite);
        }

        public void WriteOAM(ushort address, byte value)
        {
            if (_renderModeScheduler.Mode == RenderMode.OAM || _renderModeScheduler.Mode == RenderMode.Draw)
            {
                return;
            }

            int adjustedAddress = address & 0xFF;
            int index = adjustedAddress / 4;

            if (adjustedAddress % 4 == 0)
            {
                _objects[index].Y = value;
            }
            else if (adjustedAddress % 4 == 1)
            {
                _objects[index].X = value;
            }
            else if (adjustedAddress % 4 == 2)
            {
                _objects[index].Tile = value;
            }
            else
            {
                _objects[index].Flags = value;
            }
        }

        public byte ReadOAM(ushort address)
        {
            if (_renderModeScheduler.Mode == RenderMode.OAM || _renderModeScheduler.Mode == RenderMode.Draw)
            {
                return 0xFF;
            }

            int adjustedAddress = address & 0xFF;
            int index = adjustedAddress / 4;

            if (adjustedAddress % 4 == 0)
            {
                return _objects[index].Y;
            }
            else if (adjustedAddress % 4 == 1)
            {
                return _objects[index].X;
            }
            else if (adjustedAddress % 4 == 2)
            {
                return _objects[index].Tile;
            }
            else
            {
                return _objects[index].Flags;
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_spriteBuffer.Count);
            foreach (Sprite sprite in _objects)
            {
                sprite.GetState(writer);
                writer.Write(_spriteBuffer.IndexOf(sprite));
            }

            writer.Write(_objectsEnabled);
            writer.Write(_objectIndex);
            writer.Write(_largeObjects);
            writer.Write(_odma);
        }

        public void SetState(BinaryReader reader)
        {
            // GraphicsManager alters the sprites in _objects as requested by
            // the game. _spriteBuffer is always a list constructed of
            // references to Sprites in the _objects list. _spriteBuffer must
            // therefore be references to Sprites in the _objects list when
            // the state of the emulator gets reassembled
            int spriteBufferCount = reader.ReadInt32();
            Sprite[] spriteBufferArray = new Sprite[spriteBufferCount];
            foreach (Sprite sprite in _objects)
            {
                sprite.SetState(reader);
                int spriteBufferIndex = reader.ReadInt32();
                if (spriteBufferIndex >= 0)
                {
                    spriteBufferArray[spriteBufferIndex] = sprite;
                }
            }
            _spriteBuffer.Clear();
            _spriteBuffer.AddRange(spriteBufferArray);

            _objectsEnabled = reader.ReadBoolean();
            _objectIndex = reader.ReadInt32();
            _largeObjects = reader.ReadBoolean();
            _odma = reader.ReadByte();
        }
    }
}