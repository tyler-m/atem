
namespace Atem.Core.Graphics
{
    internal class GraphicsRegisters
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
                    | (_manager.WindowTileMapArea << 6)
                    | (_manager.WindowEnabled.Int() << 5)
                    | (_manager.TileDataArea << 4)
                    | (_manager.BackgroundTileMapArea << 3)
                    | (_manager.LargeObjects.Int() << 2)
                    | (_manager.ObjectsEnabled.Int() << 1)
                    | _manager.BackgroundAndWindowEnabled.Int());
            }
            set
            {
                _manager.Enabled = value.GetBit(7);
                _manager.WindowTileMapArea = value.GetBit(6).Int();
                _manager.WindowEnabled = value.GetBit(5);
                _manager.TileDataArea = value.GetBit(4).Int();
                _manager.BackgroundTileMapArea = value.GetBit(3).Int();
                _manager.LargeObjects = value.GetBit(2);
                _manager.ObjectsEnabled = value.GetBit(1);
                _manager.BackgroundAndWindowEnabled = value.GetBit(0);
            }
        }

        public byte STAT
        {
            get
            {
                return (byte)((_manager.InterruptOnLineY.Int() << 6) 
                    | (_manager.InterruptOnOAM.Int() << 5)
                    | (_manager.InterruptOnVerticalBlank.Int() << 4)
                    | (_manager.InterruptOnHorizontalBlank.Int() << 3)
                    | (_manager.CurrentlyOnLineY.Int() << 2)
                    | (int)_manager.Mode);
            }
            set
            {
                _manager.InterruptOnLineY = value.GetBit(6);
                _manager.InterruptOnOAM = value.GetBit(5);
                _manager.InterruptOnVerticalBlank = value.GetBit(4);
                _manager.InterruptOnHorizontalBlank = value.GetBit(3);
            }
        }

        public byte SCY
        {
            get
            {
                return (byte)_manager.ScreenY;
            }
            set
            {
                _manager.ScreenY = value;
            }
        }

        public byte SCX
        {
            get
            {
                return (byte)_manager.ScreenX;
            }
            set
            {
                _manager.ScreenX = value;
            }
        }

        public byte LY
        {
            get
            {
                return (byte)_manager.CurrentLine;
            }
            set {  }
        }

        public byte LYC
        {
            get
            {
                return (byte)_manager.LineYToCompare;
            }
            set
            {
                _manager.LineYToCompare = value;
            }
        }

        public byte DMA
        {
            get
            {
                return _manager.DMA;
            }
            set
            {
                _manager.DMA = value;
            }
        }

        public byte BGP
        {
            get
            {
                return _manager.BackgroundPalette;
            }
            set
            {
                _manager.BackgroundPalette = value;
            }
        }

        public byte OBP0
        {
            get
            {
                return _manager.ObjectPalette0;
            }
            set
            {
                _manager.ObjectPalette0 = value;
            }
        }

        public byte OBP1
        {
            get
            {
                return _manager.ObjectPalette1;
            }
            set
            {
                _manager.ObjectPalette1 = value;
            }
        }

        public byte WY
        {
            get
            {
                return ((byte)_manager.WindowY);
            }
            set
            {
                _manager.WindowY = value;
            }
        }

        public byte WX
        {
            get
            {
                return ((byte)_manager.WindowX);
            }
            set
            {
                _manager.WindowX = value;
            }
        }
    }
}
