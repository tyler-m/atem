using Atem.Input;
using Atem.Input.Command;

namespace Atem.Test.Input
{
    public class InputManagerTests
    {
        [Fact]
        public void Constructor_InitializesEmptyKeybindsForEachCommandType()
        {
            TestKeyProvider keyProvider = new();
            InputManager manager = new(keyProvider);

            foreach (CommandType type in Enum.GetValues(typeof(CommandType)))
            {
                Assert.NotNull(manager.Keybinds[type]);
                Assert.Empty(manager.Keybinds[type]);
            }
        }

        [Fact]
        public void AddCommand_AddsCommandToMatchingCommandType()
        {
            TestKeyProvider keyProvider = new();
            InputManager manager = new(keyProvider);

            TestCommand command = new(CommandType.A);
            manager.AddCommand(command);

            Keybind keybind = new() { Key = 1, CommandType = CommandType.A };
            manager.Keybinds[CommandType.A].Add(keybind);

            keyProvider.ActiveKeys.Add(1);
            keyProvider.KeysDown.Add(1);

            manager.Update();

            Assert.True(command.Executed);
            Assert.True(command.LastKeyDownState);
        }

        [Fact]
        public void Update_WhenBinding_AddsNewKeybindAndResetsBindingState()
        {
            TestKeyProvider keyProvider = new();
            InputManager manager = new(keyProvider);
            manager.Binding = true;
            manager.BindingType = CommandType.B;

            keyProvider.ActiveKeys.Add(42);
            keyProvider.KeysDown.Add(42);
            keyProvider.Shift = true;

            manager.Update();

            List<Keybind> keybinds = manager.Keybinds[CommandType.B];
            Assert.Single(keybinds);

            Keybind keybind = keybinds[0];
            Assert.Equal(42, keybind.Key);
            Assert.True(keybind.Shift);
            Assert.Equal(CommandType.B, keybind.CommandType);
            Assert.False(manager.Binding);
        }

        [Fact]
        public void Update_WhenRebinding_UpdatesKeybindAndResetsRebindingState()
        {
            TestKeyProvider keyProvider = new();
            InputManager manager = new(keyProvider);

            Keybind existingKeybind = new() { Key = 5, CommandType = CommandType.Start };
            manager.Rebinding = existingKeybind;

            keyProvider.ActiveKeys.Add(36);
            keyProvider.KeysDown.Add(36);
            keyProvider.Alt = true;

            manager.Update();

            Assert.Equal(36, existingKeybind.Key);
            Assert.True(existingKeybind.Alt);
            Assert.Null(manager.Rebinding);
        }

        private class TestCommand : ICommand
        {
            public bool Executed { get; private set; } = false;
            public bool LastKeyDownState { get; private set; }
            public CommandType Type { get; }

            public TestCommand(CommandType type)
            {
                Type = type;
            }

            public void Execute(bool keyDown)
            {
                Executed = true;
                LastKeyDownState = keyDown;
            }
        }

        private class TestKeyProvider : IKeyProvider
        {
            public HashSet<int> ActiveKeys { get; } = [];
            public HashSet<int> KeysDown { get; } = [];
            public IEnumerable<int> PressedKeys => ActiveKeys.Where(KeysDown.Contains);

            public bool Shift { get; set; }
            public bool Control { get; set; }
            public bool Alt { get; set; }

            public void Update() { }
            public bool IsActive(Keybind keybind) => ActiveKeys.Contains(keybind.Key);
            public bool IsKeyDown(int key) => KeysDown.Contains(key);
            public bool IsModifier(int key) => key == 16 || key == 17 || key == 18; // Shift, Ctrl, Alt

            public string GetKeyString(int keyCode) => throw new NotImplementedException();
        }
    }
}