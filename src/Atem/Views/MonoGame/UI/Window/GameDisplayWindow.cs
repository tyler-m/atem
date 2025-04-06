using ImGuiNET;
using System;
using System.Numerics;

namespace Atem.Views.MonoGame.UI.Window
{
    internal class GameDisplayWindow
    {
        private View _view;
        private IntPtr _texturePointer;
        private int _textureWidth, _textureHeight;

        public GameDisplayWindow(View view, IntPtr texturePointer, int textureWidth, int textureHeight)
        {
            _view = view;
            _texturePointer = texturePointer;
            _textureWidth = textureWidth;
            _textureHeight = textureHeight;
        }

        public void Draw()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(1, 1));
            ImGui.Begin("Game Display", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 1);
            ImGui.Image(_texturePointer, new Vector2(_textureWidth * _view.ScreenSizeFactor, _textureHeight * _view.ScreenSizeFactor));
            ImGui.End();
            ImGui.PopStyleVar();
        }
    }
}
