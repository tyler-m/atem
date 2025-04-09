namespace Atem.Config
{
    public static class ConfigDefaults
    {
        public static Config Create()
        {
            return new Config()
            {
                ScreenWidth = 160,
                ScreenHeight = 144,
                ScreenSizeFactor = 2.0f,
                Keybinds = [],
                UserVolumeFactor = 1.0f,
            };
        }
    }
}
