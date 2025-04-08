
namespace Atem.Views.MonoGame.Config
{
    public static class ViewConfigDefaults
    {
        public static ViewConfig Create()
        {
            return new ViewConfig()
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
