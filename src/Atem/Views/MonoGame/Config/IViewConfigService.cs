namespace Atem.Views.MonoGame.Config
{
    public interface IViewConfigService
    {
        public void Save(View view);
        public void Load(View view);
    }
}