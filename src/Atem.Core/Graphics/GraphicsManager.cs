using System.IO;
using Atem.Core.Graphics.Interrupts;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Screen;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core.Graphics
{
    public delegate void VerticalBlankEvent(GBColor[] screen);

    public class GraphicsManager : IStateful
    {
        public const float FrameRate = 59.73f;

        private readonly Interrupt _interrupt;
        private readonly IObjectManager _objectManager;
        private readonly ITileManager _tileManager;
        private readonly IScreenManager _screenManager;
        private readonly IHDMA _hdma;
        private readonly IRenderModeScheduler _renderModeScheduler;
        private readonly IStatInterruptDispatcher _statInterruptDispatcher;
        private readonly IPaletteProvider _paletteProvider;

        public GraphicsRegisters Registers;
        public event VerticalBlankEvent OnVerticalBlank;
        public bool Enabled { get; set; }

        public IObjectManager ObjectManager => _objectManager;
        public ITileManager TileManager => _tileManager;
        public IHDMA HDMA => _hdma;
        public IRenderModeScheduler RenderModeScheduler => _renderModeScheduler;
        public IStatInterruptDispatcher StatInterruptDispatcher => _statInterruptDispatcher;
        public IScreenManager ScreenManager => _screenManager;
        public IPaletteProvider PaletteProvider => _paletteProvider;

        public GraphicsManager(
            Interrupt interrupt, IRenderModeScheduler renderModeScheduler, IPaletteProvider paletteProvider, IHDMA hdma,
            IStatInterruptDispatcher statInterruptDispatcher, ITileManager tileManager, IObjectManager objectManager,
            IScreenManager screenManager)
        {
            _interrupt = interrupt;
            _renderModeScheduler = renderModeScheduler;
            _paletteProvider = paletteProvider;
            _hdma = hdma;
            _statInterruptDispatcher = statInterruptDispatcher;
            _tileManager = tileManager;
            _objectManager = objectManager;
            _screenManager = screenManager;

            _renderModeScheduler.RenderModeChanged += RenderModeChanged;

            Registers = new GraphicsRegisters(this);
        }

        private void RenderModeChanged(object sender, RenderModeChangedEventArgs e)
        {
            if (e.CurrentMode == RenderMode.VerticalBlank)
            {
                OnVerticalBlank?.Invoke(_screenManager.Screen);

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
            if (!Enabled)
            {
                return;
            }

            _hdma.ClockTransfer();

            if (_renderModeScheduler.Mode == RenderMode.OAM)
            {
                _objectManager.CollectObjectsForScanline();
            }
            
            _screenManager.Clock();

            _renderModeScheduler.Clock();

            _statInterruptDispatcher.UpdateLineYCompare();
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(Enabled);

            _renderModeScheduler.GetState(writer);
            _statInterruptDispatcher.GetState(writer);
            _screenManager.GetState(writer);
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
            _screenManager.SetState(reader);
            _objectManager.SetState(reader);
            _hdma.SetState(reader);
            _tileManager.SetState(reader);
            _paletteProvider.SetState(reader);
        }
    }
}
