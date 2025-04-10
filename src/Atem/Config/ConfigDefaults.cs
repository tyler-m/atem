namespace Atem.Config
{
    public static class ConfigDefaults
    {
        public static AtemConfig Create()
        {
            return new AtemConfig()
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
