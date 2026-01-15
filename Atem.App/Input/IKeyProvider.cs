namespace Atem.App.Input
{
    public interface IKeyProvider
    {
        public bool Shift { get; }
        public bool Control { get; }
        public bool Alt { get; }
        public IEnumerable<int> PressedKeys { get; }
        public void PreUpdate();
        public void PostUpdate();
        public string GetKeyString(int keyCode);
        public bool IsKeyDown(int keyCode);
        public bool IsActive(Keybind keybind);
        public bool IsModifier(int keyCode);
    }
}
