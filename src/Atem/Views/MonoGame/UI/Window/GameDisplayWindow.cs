using System;
using System.Numerics;
using ImGuiNET;

namespace Atem.Views.MonoGame.UI.Window
{
    internal class GameDisplayWindow
    {
        private static readonly Vector2 WINDOW_PADDING = new(1, 1);
        private const float WINDOW_TOP_PADDING = -1;

        private readonly IScreen _screen;
        private readonly IntPtr _texturePointer;
        private readonly int _textureWidth, _textureHeight;

        public GameDisplayWindow(IScreen screen, IntPtr texturePointer, int textureWidth, int textureHeight)
        {
            _screen = screen;
            _texturePointer = texturePointer;
            _textureWidth = textureWidth;
            _textureHeight = textureHeight;
        }

        public void Draw()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, WINDOW_PADDING);
            ImGui.Begin("Game Display", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + WINDOW_TOP_PADDING);
            ImGui.Image(_texturePointer, new Vector2(_textureWidth * _screen.SizeFactor, _textureHeight * _screen.SizeFactor));
            ImGui.End();
            ImGui.PopStyleVar();
        }
    }
}
