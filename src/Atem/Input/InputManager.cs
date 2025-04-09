using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Atem.Input.Command;

namespace Atem.Input
{
    /// <summary>
    /// A keybind is associated with a command type, and a command type is associated with commands (actions).
    /// When a key associated with a keybind is pressed, all commands of the keybind's command type are invoked.
    /// </summary>
    public class InputManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private readonly Dictionary<CommandType, List<Keybind>> _keybinds;
        private readonly Dictionary<CommandType, List<ICommand>> _commands;
        private Keybind _rebinding;
        private CommandType _bindingType;
        private bool _binding;

        public Keybind Rebinding { get => _rebinding; set => _rebinding = value; }

        public CommandType BindingType { get => _bindingType; set => _bindingType = value; }

        public bool Binding { get => _binding; set => _binding = value; }

        public Dictionary<CommandType, List<Keybind>> Keybinds
        {
            get => _keybinds;
            set
            {
                foreach ((CommandType type, List<Keybind> keybinds) in _keybinds)
                {
                    keybinds.Clear();

                    // replace old keybinds with new keybinds
                    if (value.TryGetValue(type, out List<Keybind> newKeybinds))
                    {
                        foreach (Keybind keybind in newKeybinds)
                        {
                            keybinds.Add(keybind);
                        }
                    }

                }
            }
        }

        public InputManager()
        {
            _keybinds = [];
            _commands = [];

            foreach (CommandType type in Enum.GetValues(typeof(CommandType)))
            {
                _commands.Add(type, []);
                _keybinds.Add(type, []);
            }
        }

        public void AddCommand(ICommand command)
        {
            _commands[command.Type].Add(command);
        }

        public void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            bool shiftPressed = _currentKeyboardState.IsKeyDown(Keys.LeftShift) || _currentKeyboardState.IsKeyDown(Keys.RightShift);
            bool controlPressed = _currentKeyboardState.IsKeyDown(Keys.LeftControl) || _currentKeyboardState.IsKeyDown(Keys.RightControl);
            bool altPressed = _currentKeyboardState.IsKeyDown(Keys.LeftAlt) || _currentKeyboardState.IsKeyDown(Keys.RightAlt);

            if (_rebinding != null)
            {
                HandleRebinding(shiftPressed, controlPressed, altPressed);
            }
            else if (_binding)
            {
                HandleBinding(shiftPressed, controlPressed, altPressed);
            }
            else
            {
                HandleInput(shiftPressed, controlPressed, altPressed);
            }
        }

        private void HandleInput(bool shiftPressed, bool controlPressed, bool altPressed)
        {
            foreach ((CommandType type, List<Keybind> keybinds) in _keybinds)
            {
                foreach (Keybind keybind in keybinds)
                {
                    if (_currentKeyboardState.IsKeyDown(keybind.Key) != _previousKeyboardState.IsKeyDown(keybind.Key))
                    {
                        if (keybind.Shift && !shiftPressed || keybind.Control && !controlPressed || keybind.Alt && !altPressed)
                        {
                            continue;
                        }

                        foreach (ICommand command in _commands[keybind.CommandType])
                        {
                            command.Execute(_currentKeyboardState.IsKeyDown(keybind.Key));
                        }
                    }
                }
            }
        }

        private void HandleBinding(bool shiftPressed, bool controlPressed, bool altPressed)
        {
            List<Keys> pressedKeys = _currentKeyboardState.GetPressedKeys()
                .Where(key => !_previousKeyboardState.IsKeyDown(key))
                .ToList();

            // bind if a key has been pressed
            foreach (Keys key in pressedKeys)
            {
                if (key == Keys.LeftControl || key == Keys.RightControl
                    || key == Keys.LeftAlt || key == Keys.RightAlt
                    || key == Keys.LeftShift || key == Keys.RightShift)
                {
                    continue;
                }

                Keybind keybind = new()
                {
                    Key = pressedKeys[0],
                    Shift = shiftPressed,
                    Control = controlPressed,
                    Alt = altPressed,
                    CommandType = BindingType
                };

                _keybinds[keybind.CommandType].Add(keybind);
                _binding = false;
            }
        }

        private void HandleRebinding(bool shiftPressed, bool controlPressed, bool altPressed)
        {
            List<Keys> pressedKeys = _currentKeyboardState.GetPressedKeys()
                .Where(key => !_previousKeyboardState.IsKeyDown(key))
                .ToList();

            // rebind if a key has been pressed
            foreach (Keys key in pressedKeys)
            {
                if (key == Keys.LeftControl || key == Keys.RightControl
                    || key == Keys.LeftAlt || key == Keys.RightAlt
                    || key == Keys.LeftShift || key == Keys.RightShift)
                {
                    continue;
                }

                _rebinding.Key = pressedKeys[0];
                _rebinding.Shift = shiftPressed;
                _rebinding.Control = controlPressed;
                _rebinding.Alt = altPressed;
                _rebinding = null;
            }
        }
    }
}
