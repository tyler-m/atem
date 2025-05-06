using Atem.Core.Graphics.Palettes;
using Atem.Core.State;

namespace Atem.Core.Graphics
{
    public class GraphicsRegisters : IBootable
    {
        private GraphicsManager _manager;

        public GraphicsRegisters(GraphicsManager manager)
        {
            _manager = manager;
        }

        public byte LCDC
        {
            get
            {
                return (byte)((_manager.Enabled.Int() << 7)
                    | (_manager.TileManager.WindowTileMapArea << 6)
                    | (_manager.ScreenRenderer.WindowEnabled.Int() << 5)
                    | (_manager.TileManager.TileDataArea << 4)
                    | (_manager.TileManager.BackgroundTileMapArea << 3)
                    | (_manager.ObjectManager.LargeObjects.Int() << 2)
                    | (_manager.ObjectManager.ObjectsEnabled.Int() << 1)
                    | _manager.ScreenRenderer.BackgroundAndWindowEnabledOrPriority.Int());
            }
            set
            {
                _manager.Enabled = value.GetBit(7);
                _manager.TileManager.WindowTileMapArea = value.GetBit(6).Int();
                _manager.ScreenRenderer.WindowEnabled = value.GetBit(5);
                _manager.TileManager.TileDataArea = value.GetBit(4).Int();
                _manager.TileManager.BackgroundTileMapArea = value.GetBit(3).Int();
                _manager.ObjectManager.LargeObjects = value.GetBit(2);
                _manager.ObjectManager.ObjectsEnabled = value.GetBit(1);
                _manager.ScreenRenderer.BackgroundAndWindowEnabledOrPriority = value.GetBit(0);
            }
        }

        public byte STAT
        {
            get
            {
                return (byte)((_manager.StatInterruptDispatcher.InterruptOnLineY.Int() << 6) 
                    | (_manager.StatInterruptDispatcher.InterruptOnOAM.Int() << 5)
                    | (_manager.StatInterruptDispatcher.InterruptOnVerticalBlank.Int() << 4)
                    | (_manager.StatInterruptDispatcher.InterruptOnHorizontalBlank.Int() << 3)
                    | (_manager.StatInterruptDispatcher.CurrentlyOnLineY.Int() << 2)
                    | (int)_manager.RenderModeScheduler.Mode);
            }
            set
            {
                _manager.StatInterruptDispatcher.InterruptOnLineY = value.GetBit(6);
                _manager.StatInterruptDispatcher.InterruptOnOAM = value.GetBit(5);
                _manager.StatInterruptDispatcher.InterruptOnVerticalBlank = value.GetBit(4);
                _manager.StatInterruptDispatcher.InterruptOnHorizontalBlank = value.GetBit(3);
                // CurrentlyOnLineY and Render Mode are read-only
            }
        }

        public byte SCY
        {
            get
            {
                return (byte)_manager.TileManager.ScreenY;
            }
            set
            {
                _manager.TileManager.ScreenY = value;
            }
        }

        public byte SCX
        {
            get
            {
                return (byte)_manager.TileManager.ScreenX;
            }
            set
            {
                _manager.TileManager.ScreenX = value;
            }
        }

        public byte LY
        {
            get => _manager.RenderModeScheduler.CurrentLine;
            set {  }
        }

        public byte LYC
        {
            get
            {
                return (byte)_manager.StatInterruptDispatcher.LineYToCompare;
            }
            set
            {
                _manager.StatInterruptDispatcher.LineYToCompare = value;
            }
        }

        public byte ODMA
        {
            get
            {
                return _manager.ObjectManager.ODMA;
            }
            set
            {
                _manager.ObjectManager.ODMA = value;
            }
        }

        public byte BGP
        {
            get
            {
                return _manager.PaletteProvider.DMGPalettes[0].ToDMGPalette();
            }
            set
            {
                _manager.PaletteProvider.DMGPalettes[0] = Palette.FromDMGPalette(value);
            }
        }

        public byte OBP0
        {
            get
            {
                return _manager.PaletteProvider.DMGPalettes[1].ToDMGPalette();
            }
            set
            {
                _manager.PaletteProvider.DMGPalettes[1] = Palette.FromDMGPalette(value);
            }
        }

        public byte OBP1
        {
            get
            {
                return _manager.PaletteProvider.DMGPalettes[2].ToDMGPalette();
            }
            set
            {
                _manager.PaletteProvider.DMGPalettes[2] = Palette.FromDMGPalette(value);
            }
        }

        public byte WY
        {
            get
            {
                return ((byte)_manager.TileManager.WindowY);
            }
            set
            {
                _manager.TileManager.WindowY = value;
            }
        }

        public byte WX
        {
            get
            {
                return (byte)_manager.TileManager.WindowX;
            }
            set
            {
                _manager.TileManager.WindowX = value;
            }
        }

        public byte VBK
        {
            get
            {
                return _manager.TileManager.Bank;
            }
            set
            {
                _manager.TileManager.Bank = (byte)(value & 0b1);
            }
        }

        public byte BGPI
        {
            get
            {
                return (byte)((_manager.PaletteProvider.TilePalettes.Increment.Int() << 7) | _manager.PaletteProvider.TilePalettes.Address);
            }
            set
            {
                _manager.PaletteProvider.TilePalettes.SetAddress(value.GetBit(7), value & 0b111111);
            }
        }

        public byte BGPD
        {
            get
            {
                if (_manager.RenderModeScheduler.Mode != RenderMode.Draw)
                {
                    return _manager.PaletteProvider.TilePalettes.ReadAtAddress();
                }

                return 0xFF;
            }
            set
            {
                if (_manager.RenderModeScheduler.Mode != RenderMode.Draw)
                {
                    _manager.PaletteProvider.TilePalettes.WriteAtAddress(value);
                }
            }
        }

        public byte OBPI
        {
            get
            {
                return (byte)((_manager.PaletteProvider.ObjectPalettes.Increment.Int() << 7) | _manager.PaletteProvider.ObjectPalettes.Address);
            }
            set
            {
                _manager.PaletteProvider.ObjectPalettes.SetAddress(value.GetBit(7), value & 0b111111);
            }
        }

        public byte OBPD
        {
            get
            {
                return _manager.PaletteProvider.ObjectPalettes.ReadAtAddress();
            }
            set
            {
                _manager.PaletteProvider.ObjectPalettes.WriteAtAddress(value);
            }
        }

        public byte DMA1
        {
            get
            {
                return 0xFF;
            }
            set
            {
                _manager.HDMA.SourceAddress = _manager.HDMA.SourceAddress.SetHighByte(value);
            }
        }

        public byte DMA2
        {
            get
            {
                return 0xFF;
            }
            set
            {
                // bottom 4 bits are ignored
                _manager.HDMA.SourceAddress = _manager.HDMA.SourceAddress.SetLowByte((byte)(value & 0xF0));
            }
        }

        public byte DMA3
        {
            get
            {
                return 0xFF;
            }
            set
            {
                // ensure address begins at 0x8000 (start of VRAM). top 3 bits are ignored
                _manager.HDMA.DestAddress = _manager.HDMA.DestAddress.SetHighByte((byte)((value & 0x1F) | 0x80));
            }
        }

        public byte DMA4
        {
            get
            {
                return 0xFF;
            }
            set
            {
                _manager.HDMA.DestAddress = _manager.HDMA.DestAddress.SetLowByte((byte)(value & 0xF0));
            }
        }

        public byte DMA5
        {
            get
            {
                return (byte)((((!_manager.HDMA.TransferActive).Int()) << 7) | (_manager.HDMA.TransferLengthRemaining & 0x7F));
            }
            set
            {
                _manager.HDMA.StartTransfer(value);
            }
        }

        public void Boot(BootMode mode)
        {
            switch (mode)
            {
                case BootMode.CGB:
                    LCDC = 0x91;
                    SCY = 0x00;
                    SCX = 0x00;
                    LYC = 0x00;
                    BGP = 0xFC;
                    OBP0 = 0x00;
                    OBP1 = 0x00;
                    WY = 0x00;
                    WX = 0x00;
                    break;
                case BootMode.DMG:
                    LCDC = 0x91;
                    SCY = 0x00;
                    SCX = 0x00;
                    LY = 0x00;
                    LYC = 0x00;
                    BGP = 0xFC;
                    OBP0 = 0x00;
                    OBP1 = 0x00;
                    WY = 0x00;
                    WX = 0x00;
                    break;
            }
        }
    }
}
