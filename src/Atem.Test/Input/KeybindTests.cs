using Atem.Input;
using Atem.Input.Command;

namespace Atem.Test.Input
{
    public class KeybindTests
    {
        private static Keybind CreateKeybind(CommandType type = CommandType.Start, bool shift = false, bool control = false, bool alt = false, int key = 13)
        {
            return new Keybind { CommandType = type, Shift = shift, Control = control, Alt = alt, Key = key };
        }

        [Fact]
        public void Equals_ReturnsTrue_WhenAllPropertiesMatch()
        {
            Keybind keybindA = CreateKeybind();
            Keybind keybindB = CreateKeybind();

            Assert.True(keybindA.Equals(keybindB));
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenTypeDoesNotMatch()
        {
            Keybind keybindA = CreateKeybind(type: CommandType.Start);
            Keybind keybindB = CreateKeybind(type: CommandType.Up);

            Assert.False(keybindA.Equals(keybindB));
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenShiftDoesNotMatch()
        {
            Keybind keybindA = CreateKeybind(shift: false);
            Keybind keybindB = CreateKeybind(shift: true);

            Assert.False(keybindA.Equals(keybindB));
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenControlDoesNotMatch()
        {
            Keybind keybindA = CreateKeybind(control: false);
            Keybind keybindB = CreateKeybind(control: true);

            Assert.False(keybindA.Equals(keybindB));
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenAltDoesNotMatch()
        {
            Keybind keybindA = CreateKeybind(alt: false);
            Keybind keybindB = CreateKeybind(alt: true);

            Assert.False(keybindA.Equals(keybindB));
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenKeyDoesNotMatch()
        {
            Keybind keybindA = CreateKeybind(key: 13);
            Keybind keybindB = CreateKeybind(key: 14);

            Assert.False(keybindA.Equals(keybindB));
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenOtherIsNull()
        {
            Keybind keybind = CreateKeybind();
            Assert.False(keybind.Equals(null));
        }

        [Fact]
        public void GetHashCode_ReturnsSame_WhenKeybindsAreEqual()
        {
            Keybind keybindA = CreateKeybind();
            Keybind keybindB = CreateKeybind();

            Assert.True(keybindA.Equals(keybindB));
            Assert.Equal(keybindA.GetHashCode(), keybindB.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ReturnsDifferent_WhenKeybindsAreNotEqual()
        {
            Keybind keybindA = CreateKeybind(key: 13);
            Keybind keybindB = CreateKeybind(key: 14);

            Assert.False(keybindA.Equals(keybindB));
            Assert.NotEqual(keybindA.GetHashCode(), keybindB.GetHashCode());
        }
    }
}
