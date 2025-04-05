using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Atem.Views.MonoGame.Input.Command;
using System.Linq;

namespace Atem.Views.MonoGame.Input
{
    public class InputManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private Dictionary<ICommand, HashSet<Keys>> _commands = [];
        private ICommand _rebinding;

        public ICommand Rebinding { get => _rebinding; set => _rebinding = value; }

        public Dictionary<ICommand, HashSet<Keys>> Commands { get => _commands; set => _commands = value; }

        public InputManager()
        {
            _commands = DefaultCommands();
        }

        public static Dictionary<ICommand, HashSet<Keys>> DefaultCommands()
        {
            Dictionary<ICommand, HashSet<Keys>> commands = [];
            commands.Add(new JoypadCommand(Core.Input.JoypadButton.Up), [Keys.Up]);
            commands.Add(new JoypadCommand(Core.Input.JoypadButton.Down), [Keys.Down]);
            commands.Add(new JoypadCommand(Core.Input.JoypadButton.Left), [Keys.Left]);
            commands.Add(new JoypadCommand(Core.Input.JoypadButton.Right), [Keys.Right]);
            commands.Add(new JoypadCommand(Core.Input.JoypadButton.A), [Keys.X]);
            commands.Add(new JoypadCommand(Core.Input.JoypadButton.B), [Keys.Z]);
            commands.Add(new JoypadCommand(Core.Input.JoypadButton.Start), [Keys.Enter]);
            commands.Add(new JoypadCommand(Core.Input.JoypadButton.Select), [Keys.Back]);
            commands.Add(new ExitCommand(), [Keys.Escape]);
            commands.Add(new ContinueCommand(), [Keys.F5]);
            commands.Add(new PauseCommand(), [Keys.Space]);
            return commands;
        }

        public void Update(View view)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            if (_rebinding != null)
            {
                List<Keys> pressedKeys = _currentKeyboardState.GetPressedKeys()
                    .Where(key => !_previousKeyboardState.IsKeyDown(key))
                    .ToList();

                if (pressedKeys.Count > 0)
                {
                    _commands[_rebinding].Clear();
                    _commands[_rebinding].Add(pressedKeys.First());
                    _rebinding = null;
                }
            }
            else
            {
                foreach ((ICommand command, HashSet<Keys> keys) in _commands)
                {
                    foreach (Keys key in keys)
                    {
                        if (_currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key))
                        {
                            command.Execute(view, true);
                        }
                        else if (!_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key))
                        {
                            command.Execute(view, false);
                        }
                    }
                }
            }
        }
    }
}
