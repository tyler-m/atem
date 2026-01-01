
namespace Atem.App.Config
{
    public interface IConfigService
    {
        public void SaveValues();
        public void LoadValues();
        public void SaveConfig();
        public void LoadConfig();
    }
}