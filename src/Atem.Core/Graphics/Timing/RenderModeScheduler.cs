using System;
using System.IO;
using Atem.Core.State;

namespace Atem.Core.Graphics.Timing
{
    public class RenderModeChangedEventArgs : EventArgs
    {
        public RenderMode PreviousMode { get; }
        public RenderMode CurrentMode { get; }

        public RenderModeChangedEventArgs(RenderMode previousMode, RenderMode currentMode)
        {
            PreviousMode = previousMode;
            CurrentMode = currentMode;
        }
    }

    public class RenderModeScheduler : IStateful
    {
        private const int DotsPerScanline = 456;
        private const int OAMDuration = 80;
        private const int DrawDuration = 160;
        private const byte FirstVerticalBlankLine = 144;
        private const byte LastVerticalBlankLine = 153;

        private RenderMode _mode = RenderMode.OAM;
        private byte _currentLine;
        private int _lineDotCount;

        public event EventHandler<RenderModeChangedEventArgs> RenderModeChanged;

        public byte CurrentLine => _currentLine;

        public RenderMode Mode => _mode;

        public void Clock()
        {
            _lineDotCount += 4;

            EvaluateModeTransition();
        }

        private void SetMode(RenderMode newMode)
        {
            RenderMode previousMode = _mode;
            _mode = newMode;
            RenderModeChanged?.Invoke(this, new RenderModeChangedEventArgs(previousMode, _mode));
        }

        private void EvaluateModeTransition()
        {
            if (Mode == RenderMode.HorizontalBlank)
            {
                if (_lineDotCount >= DotsPerScanline)
                {
                    _currentLine++;
                    _lineDotCount = 0;

                    if (_currentLine >= FirstVerticalBlankLine)
                    {
                        SetMode(RenderMode.VerticalBlank);
                    }
                    else
                    {
                        SetMode(RenderMode.OAM);
                    }
                }
            }
            else if (Mode == RenderMode.VerticalBlank)
            {
                if (_lineDotCount >= DotsPerScanline)
                {
                    _currentLine++;
                    _lineDotCount = 0;
                }

                if (_currentLine > LastVerticalBlankLine)
                {
                    _currentLine = 0;
                    _lineDotCount = 0;
                    SetMode(RenderMode.OAM);
                }
            }
            else if (Mode == RenderMode.OAM)
            {
                if (_lineDotCount >= OAMDuration)
                {
                    SetMode(RenderMode.Draw);
                }
            }
            else if (Mode == RenderMode.Draw)
            {
                if (_lineDotCount >= OAMDuration + DrawDuration)
                {
                    SetMode(RenderMode.HorizontalBlank);
                }
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write((byte)_mode);
            writer.Write(_currentLine);
            writer.Write(_lineDotCount);
        }

        public void SetState(BinaryReader reader)
        {
            _mode = (RenderMode)reader.ReadByte();
            _currentLine = reader.ReadByte();
            _lineDotCount = reader.ReadInt32();
        }
    }
}
