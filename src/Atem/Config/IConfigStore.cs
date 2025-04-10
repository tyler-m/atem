namespace Atem.Config
{
    public interface IConfigStore
    {
        public void Save(AtemConfig config);
        public AtemConfig Load();
    }
}
