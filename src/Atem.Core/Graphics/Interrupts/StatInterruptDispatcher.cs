﻿using Atem.Core.Graphics.Timing;
using Atem.Core.Processing;
using System.IO;

namespace Atem.Core.Graphics.Interrupts
{
    public class StatInterruptDispatcher : IStatInterruptDispatcher
    {
        private readonly Interrupt _interrupt;
        private readonly IRenderModeScheduler _scheduler;
        private bool _interruptOnOAM;
        private bool _interruptOnVerticalBlank;
        private bool _interruptOnHorizontalBlank;
        private bool _interruptOnLineY;
        private byte _lineYToCompare;
        private bool _currentlyOnLineY;

        public bool InterruptOnOAM { get => _interruptOnOAM; set => _interruptOnOAM = value; }
        public bool InterruptOnVerticalBlank { get => _interruptOnVerticalBlank; set => _interruptOnVerticalBlank = value; }
        public bool InterruptOnHorizontalBlank { get => _interruptOnHorizontalBlank; set => _interruptOnHorizontalBlank = value; }
        public bool InterruptOnLineY { get => _interruptOnLineY; set => _interruptOnLineY = value; }
        public byte LineYToCompare { get => _lineYToCompare; set => _lineYToCompare = value; }
        public bool CurrentlyOnLineY => _currentlyOnLineY;

        public StatInterruptDispatcher(Interrupt interrupt, IRenderModeScheduler scheduler)
        {
            _interrupt = interrupt;
            _scheduler = scheduler;

            _scheduler.RenderModeChanged += HandleRenderModeChanged;
        }

        public void UpdateLineYCompare()
        {
            bool wasMatching = _currentlyOnLineY;
            _currentlyOnLineY = _lineYToCompare == _scheduler.CurrentLine;

            if (_interruptOnLineY && _currentlyOnLineY && !wasMatching)
            {
                _interrupt.SetInterrupt(InterruptType.STAT);
            }
        }

        private void HandleRenderModeChanged(object sender, RenderModeChangedEventArgs e)
        {
            if (e.CurrentMode == RenderMode.OAM && _interruptOnOAM)
            {
                _interrupt.SetInterrupt(InterruptType.STAT);
            }
            else if (e.CurrentMode == RenderMode.VerticalBlank && _interruptOnVerticalBlank)
            {
                _interrupt.SetInterrupt(InterruptType.STAT);
            }
            else if (e.CurrentMode == RenderMode.HorizontalBlank && _interruptOnHorizontalBlank)
            {
                _interrupt.SetInterrupt(InterruptType.STAT);
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_lineYToCompare);
            writer.Write(_currentlyOnLineY);
            writer.Write(_interruptOnLineY);
            writer.Write(_interruptOnOAM);
            writer.Write(_interruptOnVerticalBlank);
            writer.Write(_interruptOnHorizontalBlank);
        }

        public void SetState(BinaryReader reader)
        {
            _lineYToCompare = reader.ReadByte();
            _currentlyOnLineY = reader.ReadBoolean();
            _interruptOnLineY = reader.ReadBoolean();
            _interruptOnOAM = reader.ReadBoolean();
            _interruptOnVerticalBlank = reader.ReadBoolean();
            _interruptOnHorizontalBlank = reader.ReadBoolean();
        }
    }
}