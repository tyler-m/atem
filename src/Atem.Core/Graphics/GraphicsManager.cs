using System.Collections.Generic;
using System.IO;
using Atem.Core.Graphics.Interrupts;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Screen;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core.Graphics
{
    public delegate void VerticalBlankEvent(GBColor[] screen);

    public class GraphicsManager : IMemoryProvider, IStateful
    {
        public const float FrameRate = 59.73f;

        private readonly Interrupt _interrupt;
        private readonly IObjectManager _objectManager;
        private readonly ITileManager _tileManager;
        private readonly IScreenRenderer _screenRenderer;
        private readonly IHDMA _hdma;
        private readonly IRenderModeScheduler _renderModeScheduler;
        private readonly IStatInterruptDispatcher _statInterruptDispatcher;
        private readonly IPaletteProvider _paletteProvider;

        public GraphicsRegisters Registers;
        public event VerticalBlankEvent OnVerticalBlank;

        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled && !value)
                {
                    _renderModeScheduler.Stop();
                }
                else if (!_enabled && value)
                {
                    _renderModeScheduler.Resume();
                }

                _enabled = value;
            }
        }

        public IObjectManager ObjectManager => _objectManager;
        public ITileManager TileManager => _tileManager;
        public IHDMA HDMA => _hdma;
        public IRenderModeScheduler RenderModeScheduler => _renderModeScheduler;
        public IStatInterruptDispatcher StatInterruptDispatcher => _statInterruptDispatcher;
        public IScreenRenderer ScreenRenderer => _screenRenderer;
        public IPaletteProvider PaletteProvider => _paletteProvider;

        public GraphicsManager(
            Interrupt interrupt, IRenderModeScheduler renderModeScheduler, IPaletteProvider paletteProvider, IHDMA hdma,
            IStatInterruptDispatcher statInterruptDispatcher, ITileManager tileManager, IObjectManager objectManager,
            IScreenRenderer screenRenderer)
        {
            _interrupt = interrupt;
            _renderModeScheduler = renderModeScheduler;
            _paletteProvider = paletteProvider;
            _hdma = hdma;
            _statInterruptDispatcher = statInterruptDispatcher;
            _tileManager = tileManager;
            _objectManager = objectManager;
            _screenRenderer = screenRenderer;

            _renderModeScheduler.RenderModeChanged += RenderModeChanged;

            Registers = new GraphicsRegisters(this);
        }

        private void RenderModeChanged(object sender, RenderModeChangedEventArgs e)
        {
            if (e.CurrentMode == RenderMode.VerticalBlank)
            {
                OnVerticalBlank?.Invoke(_screenRenderer.Screen);

                _interrupt.SetInterrupt(InterruptType.VerticalBlank);
            }
        }

        public void WriteVRAM(ushort address, byte value, bool ignoreRenderMode = false)
        {
            _tileManager.WriteVRAM(address, value, ignoreRenderMode);
        }

        public byte ReadVRAM(ushort address)
        {
            return _tileManager.ReadVRAM(address);
        }

        public void WriteOAM(ushort address, byte value)
        {
            _objectManager.WriteOAM(address, value);
        }

        public byte ReadOAM(ushort address)
        {
            return _objectManager.ReadOAM(address);
        }

        public void Clock()
        {
            _hdma.ClockTransfer();

            if (!Enabled)
            {
                return;
            }

            if (_renderModeScheduler.Mode == RenderMode.OAM)
            {
                _objectManager.CollectObjectsForScanline();
            }
            
            _screenRenderer.Clock();

            _renderModeScheduler.Clock();

            _statInterruptDispatcher.UpdateLineYCompare();
        }

        public byte ReadRegister(ushort address)
        {
            return address switch
            {
                0xFF40 => Registers.LCDC,
                0xFF41 => Registers.STAT,
                0xFF42 => Registers.SCY,
                0xFF43 => Registers.SCX,
                0xFF44 => Registers.LY,
                0xFF45 => Registers.LYC,
                0xFF46 => Registers.ODMA,
                0xFF47 => Registers.BGP,
                0xFF48 => Registers.OBP0,
                0xFF49 => Registers.OBP1,
                0xFF4A => Registers.WY,
                0xFF4B => Registers.WX,
                0xFF4F => Registers.VBK,
                0xFF51 => Registers.DMA1,
                0xFF52 => Registers.DMA2,
                0xFF53 => Registers.DMA3,
                0xFF54 => Registers.DMA4,
                0xFF55 => Registers.DMA5,
                0xFF68 => Registers.BGPI,
                0xFF69 => Registers.BGPD,
                0xFF6A => Registers.OBPI,
                0xFF6B => Registers.OBPD,
                _ => 0xFF,
            };
        }

        public void WriteRegister(ushort address, byte value)
        {
            switch (address)
            {
                case 0xFF40:
                    Registers.LCDC = value;
                    break;
                case 0xFF41:
                    Registers.STAT = value;
                    break;
                case 0xFF42:
                    Registers.SCY = value;
                    break;
                case 0xFF43:
                    Registers.SCX = value;
                    break;
                case 0xFF45:
                    Registers.LYC = value;
                    break;
                case 0xFF46:
                    Registers.ODMA = value;
                    break;
                case 0xFF47:
                    Registers.BGP = value;
                    break;
                case 0xFF48:
                    Registers.OBP0 = value;
                    break;
                case 0xFF49:
                    Registers.OBP1 = value;
                    break;
                case 0xFF4A:
                    Registers.WY = value;
                    break;
                case 0xFF4B:
                    Registers.WX = value;
                    break;
                case 0xFF4F:
                    Registers.VBK = value;
                    break;
                case 0xFF51:
                    Registers.DMA1 = value;
                    break;
                case 0xFF52:
                    Registers.DMA2 = value;
                    break;
                case 0xFF53:
                    Registers.DMA3 = value;
                    break;
                case 0xFF54:
                    Registers.DMA4 = value;
                    break;
                case 0xFF55:
                    Registers.DMA5 = value;
                    break;
                case 0xFF68:
                    Registers.BGPI = value;
                    break;
                case 0xFF69:
                    Registers.BGPD = value;
                    break;
                case 0xFF6A:
                    Registers.OBPI = value;
                    break;
                case 0xFF6B:
                    Registers.OBPD = value;
                    break;
            }
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            if (address <= 0x9FFF)
            {
                return ReadVRAM(address);
            }
            else if (address <= 0xFE9F)
            {
                return ReadOAM(address);
            }
            else
            {
                return ReadRegister(address);
            }
        }

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false)
        {
            if (address <= 0x9FFF)
            {
                WriteVRAM(address, value, ignoreAccessRestrictions);
            }
            else if (address <= 0xFE9F)
            {
                WriteOAM(address, value);
            }
            else
            {
                WriteRegister(address, value);
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetMemoryRanges()
        {
            yield return (0x8000, 0x9FFF); // VRAM
            yield return (0xFE00, 0xFE9F); // OAM
            yield return (0xFF40, 0xFF4B); // registers
            yield return (0xFF4F, 0xFF4F);
            yield return (0xFF51, 0xFF55);
            yield return (0xFF68, 0xFF6B);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(Enabled);

            _renderModeScheduler.GetState(writer);
            _statInterruptDispatcher.GetState(writer);
            _screenRenderer.GetState(writer);
            _objectManager.GetState(writer);
            _hdma.GetState(writer);
            _tileManager.GetState(writer);
            _paletteProvider.GetState(writer);
        }

        public void SetState(BinaryReader reader)
        {
            Enabled = reader.ReadBoolean();

            _renderModeScheduler.SetState(reader);
            _statInterruptDispatcher.SetState(reader);
            _screenRenderer.SetState(reader);
            _objectManager.SetState(reader);
            _hdma.SetState(reader);
            _tileManager.SetState(reader);
            _paletteProvider.SetState(reader);
        }
    }
}
