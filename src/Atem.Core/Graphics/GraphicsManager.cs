﻿using System.IO;
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

        private readonly IBus _bus;
        private readonly ObjectManager _objectManager;
        private readonly TileManager _tileManager;
        private readonly ScreenManager _screenManager;
        private readonly HDMA _hdma;
        private readonly RenderModeScheduler _renderModeScheduler;
        private readonly StatInterruptManager _statInterruptManager;

        public GraphicsRegisters Registers;
        public event VerticalBlankEvent OnVerticalBlank;
        public bool Enabled;
        public bool WindowEnabled;
        public bool BackgroundAndWindowEnabledOrPriority;
        public int ScreenX;
        public int ScreenY;
        public int WindowX;
        public int WindowY;
        public PaletteGroup DMGPalettes = new();

        public ObjectManager ObjectManager { get => _objectManager; }
        public TileManager TileManager { get => _tileManager; }
        public HDMA HDMA { get => _hdma; }
        public RenderModeScheduler RenderModeScheduler { get => _renderModeScheduler; }
        public StatInterruptManager StatInterruptManager { get => _statInterruptManager; }

        public GraphicsManager(IBus bus)
        {
            _bus = bus;
            _tileManager = new TileManager(bus);
            _renderModeScheduler = new RenderModeScheduler();
            _screenManager = new ScreenManager(bus, _renderModeScheduler);
            _objectManager = new ObjectManager(bus, _renderModeScheduler);
            _hdma = new(bus, _renderModeScheduler);
            _renderModeScheduler.RenderModeChanged += RenderModeChanged;
            _statInterruptManager = new StatInterruptManager(bus, _renderModeScheduler);
            Registers = new GraphicsRegisters(this);
        }

        private void RenderModeChanged(object sender, RenderModeChangedEventArgs e)
        {
            if (e.CurrentMode == RenderMode.VerticalBlank)
            {
                OnVerticalBlank?.Invoke(_screenManager.Screen);

                _bus.RequestInterrupt(InterruptType.VerticalBlank);
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

            _statInterruptManager.UpdateLineYCompare();
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(Enabled);
            writer.Write(WindowEnabled);
            writer.Write(BackgroundAndWindowEnabledOrPriority);
            writer.Write(WindowX);
            writer.Write(WindowY);
            writer.Write(ScreenX);
            writer.Write(ScreenY);
            DMGPalettes.GetState(writer);

            _renderModeScheduler.GetState(writer);
            _statInterruptManager.GetState(writer);
            _screenManager.GetState(writer);
            _objectManager.GetState(writer);
            _hdma.GetState(writer);
            _tileManager.GetState(writer);
        }

        public void SetState(BinaryReader reader)
        {
            Enabled = reader.ReadBoolean();
            WindowEnabled = reader.ReadBoolean();
            BackgroundAndWindowEnabledOrPriority = reader.ReadBoolean();
            WindowX = reader.ReadInt32();
            WindowY = reader.ReadInt32();
            ScreenX = reader.ReadInt32();
            ScreenY = reader.ReadInt32();
            DMGPalettes.SetState(reader);

            _renderModeScheduler.SetState(reader);
            _statInterruptManager.SetState(reader);
            _screenManager.SetState(reader);
            _objectManager.SetState(reader);
            _hdma.SetState(reader);
            _tileManager.SetState(reader);
        }
    }
}
