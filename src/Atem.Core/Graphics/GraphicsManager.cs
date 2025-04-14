using System.IO;
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

        private readonly IBus _bus;
        private readonly ObjectManager _objectManager;
        private readonly TileManager _tileManager;
        private readonly ScreenManager _screenManager;
        private readonly HDMA _hdma;
        private readonly RenderModeScheduler _renderModeScheduler;
        private readonly StatInterruptManager _statInterruptManager;
        private bool _windowWasTriggeredThisFrame;
        private bool _justEnteredHorizontalBlank;

        public GraphicsRegisters Registers;
        public event VerticalBlankEvent OnVerticalBlank;
        public bool Enabled;
        public bool WindowEnabled;
        public bool BackgroundAndWindowEnabledOrPriority;
        public int ScreenX;
        public int ScreenY;
        public int WindowX;
        public int WindowY;
        public byte CurrentWindowLine;
        public PaletteGroup DMGPalettes = new();

        public ObjectManager ObjectManager { get => _objectManager; }
        public TileManager TileManager { get => _tileManager; }
        public ScreenManager ScreenManager { get => _screenManager; }
        public HDMA HDMA { get => _hdma; }
        public RenderModeScheduler RenderModeScheduler { get => _renderModeScheduler; }
        public bool WindowWasTriggeredThisFrame { get => _windowWasTriggeredThisFrame; set => _windowWasTriggeredThisFrame = value; }
        public StatInterruptManager StatInterruptManager { get => _statInterruptManager; }

        public GraphicsManager(IBus bus)
        {
            _bus = bus;
            _hdma = new(bus);
            _objectManager = new ObjectManager(bus);
            _tileManager = new TileManager(bus);
            _screenManager = new ScreenManager(bus);
            _renderModeScheduler = new RenderModeScheduler();
            _renderModeScheduler.RenderModeChanged += RenderModeChanged;
            _statInterruptManager = new StatInterruptManager(bus, _renderModeScheduler);
            Registers = new GraphicsRegisters(this);
        }

        private void RenderModeChanged(object sender, RenderModeChangedEventArgs e)
        {
            if (e.CurrentMode == RenderMode.HorizontalBlank)
            {
                OnHorizontalBlankStart(e.PreviousMode);
            }
            else if (e.CurrentMode == RenderMode.OAM)
            {
                OnOAMStart(e.PreviousMode);
            }
            else if (e.CurrentMode == RenderMode.VerticalBlank)
            {
                OnVerticalBlankStart(e.PreviousMode);
            }
        }

        private void OnHorizontalBlankStart(RenderMode previousMode)
        {
            if (previousMode == RenderMode.Draw)
            {
                if (_windowWasTriggeredThisFrame)
                {
                    CurrentWindowLine++;
                }

                _windowWasTriggeredThisFrame = false;
                _hdma.JustEnteredHorizontalBlank = true;
            }
        }

        private void OnOAMStart(RenderMode previousMode)
        {
            _objectManager.ResetScanline();

            if (previousMode == RenderMode.VerticalBlank)
            {
                CurrentWindowLine = 0;
            }
        }

        private void OnVerticalBlankStart(RenderMode previousMode)
        {
            OnVerticalBlank?.Invoke(_screenManager.Screen);

            _bus.RequestInterrupt(InterruptType.VerticalBlank);
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
            if (_renderModeScheduler.Mode == RenderMode.OAM || _renderModeScheduler.Mode == RenderMode.Draw)
            {
                return;
            }

            _objectManager.WriteOAM(address, value);
        }

        public byte ReadOAM(ushort address)
        {
            if (_renderModeScheduler.Mode == RenderMode.OAM || _renderModeScheduler.Mode == RenderMode.Draw)
            {
                return 0xFF;
            }

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
                _objectManager.CollectObjectsForScanline(_renderModeScheduler.CurrentLine);
            }
            
            _screenManager.Clock();

            _renderModeScheduler.Clock();

            _statInterruptManager.UpdateLineYCompare();
        }

        public void GetState(BinaryWriter writer)
        {
            _renderModeScheduler.GetState(writer);

            _statInterruptManager.GetState(writer);

            writer.Write(_tileManager.VRAM);

            _screenManager.GetState(writer);

            _objectManager.GetState(writer);

            writer.Write(_windowWasTriggeredThisFrame);
            writer.Write(_justEnteredHorizontalBlank);

            writer.Write(Enabled);
            writer.Write(_tileManager.WindowTileMapArea);
            writer.Write(_tileManager.BackgroundTileMapArea);
            writer.Write(_tileManager.TileDataArea);
            writer.Write(WindowEnabled);
            writer.Write(BackgroundAndWindowEnabledOrPriority);
            writer.Write(ScreenX);
            writer.Write(ScreenY);
            writer.Write(WindowX);
            writer.Write(WindowY);
            writer.Write(CurrentWindowLine);

            _tileManager.TilePalettes.GetState(writer);
            DMGPalettes.GetState(writer);

            _hdma.GetState(writer);

            writer.Write(_tileManager.Bank);
        }

        public void SetState(BinaryReader reader)
        {
            _renderModeScheduler.SetState(reader);

            _statInterruptManager.SetState(reader);

            _tileManager.VRAM = reader.ReadBytes(_tileManager.VRAM.Length);

            _screenManager.SetState(reader);

            _objectManager.SetState(reader);

            _windowWasTriggeredThisFrame = reader.ReadBoolean();
            _justEnteredHorizontalBlank = reader.ReadBoolean();

            Enabled = reader.ReadBoolean();
            _tileManager.WindowTileMapArea = reader.ReadInt32();
            _tileManager.BackgroundTileMapArea = reader.ReadInt32();
            _tileManager.TileDataArea = reader.ReadInt32();
            WindowEnabled = reader.ReadBoolean();
            BackgroundAndWindowEnabledOrPriority = reader.ReadBoolean();
            ScreenX = reader.ReadInt32();
            ScreenY = reader.ReadInt32();
            WindowX = reader.ReadInt32();
            WindowY = reader.ReadInt32();
            CurrentWindowLine = reader.ReadByte();

            _tileManager.TilePalettes.SetState(reader);
            DMGPalettes.SetState(reader);

            _hdma.SetState(reader);

            _tileManager.Bank = reader.ReadByte();
        }
    }
}
