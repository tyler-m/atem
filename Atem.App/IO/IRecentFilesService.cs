namespace Atem.App.IO
{
    public interface IRecentFilesService
    {
        void Add(string filePath);
        IEnumerable<string> RecentFiles { get; set; }
        IEnumerable<FileInfo> RecentFilesInfo { get; }
    }
}
