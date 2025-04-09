using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Atem.Input;

namespace Atem.Views.MonoGame.Input
{
    internal class KeyProvider : IKeyProvider
    {
        private KeyboardState _currentState;
        private KeyboardState _previousState;

        public bool Shift { get => IsKeyDown((int)Keys.LeftShift) || IsKeyDown((int)Keys.RightShift); }

        public bool Control { get => IsKeyDown((int)Keys.LeftControl) || IsKeyDown((int)Keys.RightControl); }

        public bool Alt { get => IsKeyDown((int)Keys.LeftAlt) || IsKeyDown((int)Keys.RightAlt); }

        public IEnumerable<int> PressedKeys
        {
            get
            {
                return _currentState.GetPressedKeys().Select(key => (int)key).Where(WasKeyUp);
            }
        }

        public bool IsKeyDown(int keyCode)
        {
            return _currentState.IsKeyDown((Keys)keyCode);
        }

        private bool WasKeyUp(int keyCode)
        {
            return _previousState.IsKeyUp((Keys)keyCode);
        }

        public bool DidKeyChange(int keyCode)
        {
            return _currentState.IsKeyDown((Keys)keyCode) != _previousState.IsKeyDown((Keys)keyCode);
        }

        public void Update()
        {
            _previousState = _currentState;
            _currentState = Keyboard.GetState();
        }

        public bool IsActive(Keybind keybind)
        {
            if (keybind.Shift && !Shift || keybind.Control && !Control || keybind.Alt && !Alt)
            {
                return false;
            }

            return DidKeyChange(keybind.Key);
        }

        public string GetKeyString(int keyCode)
        {
            return ((Keys)keyCode).ToString();
        }

        public bool IsModifier(int keyCode)
        {
            return keyCode >= (int)Keys.LeftShift && keyCode <= (int)Keys.RightAlt;
        }
    }
}
