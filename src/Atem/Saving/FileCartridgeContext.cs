namespace Atem.Saving
{
    internal class FileCartridgeContext : ICartridgeContext
    {
        private string _filePath;

        public string Id { get => _filePath; set => _filePath = value; }
    }
}
