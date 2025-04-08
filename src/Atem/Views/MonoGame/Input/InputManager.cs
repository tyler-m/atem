using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Atem.Views.MonoGame.Input.Command;

namespace Atem.Views.MonoGame.Input
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

            if (_rebinding != null)
            {
                HandleRebinding(shiftPressed);
            }
            else if (_binding)
            {
                HandleBinding(shiftPressed);
            }
            else
            {
                HandleInput(shiftPressed);
            }
        }

        private void HandleInput(bool shiftPressed)
        {
            foreach ((CommandType type, List<Keybind> keybinds) in _keybinds)
            {
                foreach (Keybind keybind in keybinds)
                {
                    if (_currentKeyboardState.IsKeyDown(keybind.Key) != _previousKeyboardState.IsKeyDown(keybind.Key))
                    {
                        if (keybind.Shift && !shiftPressed)
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

        private void HandleBinding(bool shiftPressed)
        {
            List<Keys> pressedKeys = _currentKeyboardState.GetPressedKeys()
                .Where(key => !_previousKeyboardState.IsKeyDown(key))
                .ToList();

            // bind if a key has been pressed
            if (pressedKeys.Count > 0)
            {
                Keybind keybind = new Keybind();
                keybind.Key = pressedKeys[0];
                keybind.Shift = shiftPressed;
                keybind.CommandType = BindingType;
                _keybinds[keybind.CommandType].Add(keybind);
                _binding = false;
            }
        }

        private void HandleRebinding(bool shiftPressed)
        {
            List<Keys> pressedKeys = _currentKeyboardState.GetPressedKeys()
                .Where(key => !_previousKeyboardState.IsKeyDown(key))
                .ToList();

            // rebind if a key has been pressed
            if (pressedKeys.Count > 0)
            {
                _rebinding.Shift = shiftPressed;
                _rebinding.Key = pressedKeys[0];
                _rebinding = null;
            }
        }
    }
}
