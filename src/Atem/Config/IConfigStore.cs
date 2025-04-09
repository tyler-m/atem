namespace Atem.Config
{
    public interface IConfigStore
    {
        public void Save(Config config);
        public Config Load();
    }
}
