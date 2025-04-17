using Atem.IO;

namespace Atem.Test.IO
{
    public class RecentFilesServiceTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly List<string> _testFilePaths = [];
        private readonly RecentFilesService _service;

        public RecentFilesServiceTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _service = new RecentFilesService();
        }

        private string CreateTestFile(string fileName)
        {
            string path = Path.Combine(_testDirectory, fileName);
            File.WriteAllText(path, string.Empty);
            _testFilePaths.Add(path);
            return path;
        }

        [Fact]
        public void Add_WhenFileIsAdded_ShouldBeInRecentFiles()
        {
            string filePath = CreateTestFile("file1");

            _service.Add(filePath);

            Assert.Single(_service.RecentFiles);
            Assert.Equal(filePath, _service.RecentFiles.First());
        }

        [Fact]
        public void Add_WhenDuplicateFileIsAdded_ShouldMoveItToTop()
        {
            string filePath1 = CreateTestFile("file1");
            string filePath2 = CreateTestFile("file2");

            _service.Add(filePath1);
            _service.Add(filePath2);
            _service.Add(filePath1); // re-add to move it to top

            List<string> recentFiles = _service.RecentFiles.ToList();

            Assert.Equal(2, recentFiles.Count);
            Assert.Equal(filePath1, recentFiles.ElementAt(0));
            Assert.Equal(filePath2, recentFiles.ElementAt(1));
        }

        [Fact]
        public void Add_WhenMoreThanMaxFiles_ShouldRemoveOldest()
        {
            for (int i = 0; i < RecentFilesService.MaxFiles + 1; i++)
            {
                string filePath = CreateTestFile($"file{i}");
                _service.Add(filePath);
            }

            List<string> recentFiles = _service.RecentFiles.ToList();

            Assert.Equal(RecentFilesService.MaxFiles, recentFiles.Count);
            Assert.DoesNotContain(_testFilePaths.First(), recentFiles); // first file was removed
        }

        [Fact]
        public void RecentFiles_Setter_ShouldOverwriteList()
        {
            string filePath1 = CreateTestFile("file1");
            string filePath2 = CreateTestFile("file2");

            _service.Add(filePath1);
            _service.RecentFiles = [filePath2];

            Assert.Single(_service.RecentFiles);
            Assert.Contains(filePath2, _service.RecentFiles);
        }

        [Fact]
        public void Add_WhenPathCaseDiffers_ShouldTreatPathsAsEqual()
        {
            string filePathNameLower = CreateTestFile("file");
            string filePathNameUpper = Path.Combine(_testDirectory, "FILE");
            _service.Add(filePathNameLower);

            // simulate different casing
            File.Move(filePathNameLower, filePathNameUpper);
            _testFilePaths.Add(filePathNameUpper);

            _service.Add(filePathNameUpper);

            List<string> recent = _service.RecentFiles.ToList();

            Assert.Single(recent);
            Assert.Equal(filePathNameUpper, recent.First());
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}