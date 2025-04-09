using System;
using System.Collections.Generic;
using Atem.Input.Command;

namespace Atem.Input
{
    /// <summary>
    /// A keybind is associated with a command type, and a command type is associated with commands (actions).
    /// When a key associated with a keybind is pressed, all commands of the keybind's command type are invoked.
    /// </summary>
    public class InputManager
    {
        private readonly Dictionary<CommandType, List<Keybind>> _keybinds;
        private readonly Dictionary<CommandType, List<ICommand>> _commands;
        private readonly IKeyProvider _keyProvider;
        private Keybind _rebinding;
        private CommandType _bindingType;
        private bool _binding;

        public IKeyProvider KeyProvider { get => _keyProvider; }
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

        public InputManager(IKeyProvider keyProvider)
        {
            _keybinds = [];
            _commands = [];

            foreach (CommandType type in Enum.GetValues(typeof(CommandType)))
            {
                _commands.Add(type, []);
                _keybinds.Add(type, []);
            }

            _keyProvider = keyProvider;
        }

        public void AddCommand(ICommand command)
        {
            _commands[command.Type].Add(command);
        }

        public void Update()
        {
            _keyProvider.Update();

            if (_rebinding != null)
            {
                HandleRebinding();
            }
            else if (_binding)
            {
                HandleBinding();
            }
            else
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            foreach ((CommandType type, List<Keybind> keybinds) in _keybinds)
            {
                foreach (Keybind keybind in keybinds)
                {
                    if (_keyProvider.IsActive(keybind))
                    {
                        foreach (ICommand command in _commands[keybind.CommandType])
                        {
                            command.Execute(_keyProvider.IsKeyDown(keybind.Key));
                        }
                    }
                }
            }
        }

        private void HandleBinding()
        {   
            // bind if a key has been pressed
            foreach (int key in _keyProvider.PressedKeys)
            {
                // ignore if the key is a modifier
                if (_keyProvider.IsModifier(key))
                {
                    continue;
                }

                Keybind keybind = new()
                {
                    Key = key,
                    Shift = _keyProvider.Shift,
                    Control = _keyProvider.Control,
                    Alt = _keyProvider.Alt,
                    CommandType = BindingType
                };

                _keybinds[keybind.CommandType].Add(keybind);
                _binding = false;
            }
        }

        private void HandleRebinding()
        {
            // rebind if a key has been pressed
            foreach (int key in _keyProvider.PressedKeys)
            {
                // ignore if the key is a modifier
                if (_keyProvider.IsModifier(key))
                {
                    continue;
                }

                _rebinding.Key = key;
                _rebinding.Shift = _keyProvider.Shift;
                _rebinding.Control = _keyProvider.Control;
                _rebinding.Alt = _keyProvider.Alt;
                _rebinding = null;
            }
        }
    }
}
