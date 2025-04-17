using Atem.IO;

namespace Atem.Test.IO
{
    public class FileBrowserTests : IDisposable
    {
        private const int NumberOfFilesInStartingDirectory = 2;

        private readonly DirectoryInfo _startingDirectory;
        private readonly DirectoryInfo _subDirectory;
        private readonly FileInfo _file1;
        private readonly FileInfo _file2;

        public FileBrowserTests()
        {
            string startDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _startingDirectory = Directory.CreateDirectory(startDirectoryPath);
            
            string subDirectoryPath = Path.Combine(_startingDirectory.FullName, "sub directory");
            _subDirectory = Directory.CreateDirectory(subDirectoryPath);

            string file1Path = Path.Combine(_startingDirectory.FullName, "file1.gb");
            File.WriteAllText(file1Path, "1");
            _file1 = new FileInfo(file1Path);


            string file2Path = Path.Combine(_startingDirectory.FullName, "file2.gbc");
            File.WriteAllText(file2Path, "2");
            _file2 = new FileInfo(file2Path);
        }

        [Fact]
        public void Constructor_ShouldSetCurrentDirectoryToStartingDirectory()
        {
            FileBrowser fileBrowser = new(_startingDirectory.FullName);

            Assert.Equal(_startingDirectory.FullName, fileBrowser.CurrentDirectory.FullName);
        }

        [Fact]
        public void NavigateTo_ShouldChangeCurrentDirectory()
        {
            FileBrowser fileBrowser = new(_startingDirectory.FullName);

            fileBrowser.NavigateTo(_subDirectory);

            Assert.Equal(_subDirectory.FullName, fileBrowser.CurrentDirectory.FullName);
        }

        [Fact]
        public void NavigateUp_WhenCurrentDirectoryHasParent_ShouldNavigateToParent()
        {
            FileBrowser fileBrowser = new(_subDirectory.FullName);

            fileBrowser.NavigateUp();

            Assert.Equal(_startingDirectory.FullName, fileBrowser.CurrentDirectory.FullName);
        }

        [Fact]
        public void NavigateUp_WhenAtRoot_ShouldRemainAtCurrentDirectory()
        {
            FileBrowser fileBrowser = new(_startingDirectory.FullName);
            while (fileBrowser.CurrentDirectory.Parent != null)
            {
                fileBrowser.NavigateUp();
            }
            string currentDirectoryPath = fileBrowser.CurrentDirectory.FullName;

            // attempt to navigate up when parent is null
            Assert.Null(fileBrowser.CurrentDirectory.Parent);
            fileBrowser.NavigateUp();

            Assert.Equal(currentDirectoryPath, fileBrowser.CurrentDirectory.FullName);
        }

        [Fact]
        public void GetDirectories_ShouldReturnSubDirectories()
        {
            FileBrowser fileBrowser = new(_startingDirectory.FullName);

            List<DirectoryInfo> directories = fileBrowser.GetDirectories().ToList();

            Assert.Single(directories);
            Assert.Equal(_subDirectory.FullName, directories[0].FullName);
        }

        [Fact]
        public void GetFiles_WhenFilteredByExtension_ShouldReturnMatchingFiles()
        {
            FileBrowser fileBrowser = new(_startingDirectory.FullName);

            List<FileInfo> files = fileBrowser.GetFiles([_file1.Extension]).ToList();

            Assert.Single(files);
            Assert.Equal(_file1.Name, files.First().Name);
        }

        [Fact]
        public void GetFiles_WhenFilteredByMultipleExtensions_ShouldReturnAllMatches()
        {
            FileBrowser fileBrowser = new(_startingDirectory.FullName);
            List<FileInfo> files = fileBrowser.GetFiles([_file1.Extension, _file2.Extension]).ToList();

            Assert.Equal(NumberOfFilesInStartingDirectory, files.Count);
            string[] fileNames = files.Select(fileInfo => fileInfo.Name).ToArray();
            Assert.Contains(_file1.Name, fileNames);
            Assert.Contains(_file2.Name, fileNames);
        }

        [Fact]
        public void GetFiles_WhenNoExtensionsProvided_ShouldReturnAllFiles()
        {
            FileBrowser fileBrowser = new(_startingDirectory.FullName);
            List<FileInfo> files = fileBrowser.GetFiles().ToList();

            Assert.Equal(NumberOfFilesInStartingDirectory, files.Count);
            string[] fileNames = files.Select(fileInfo => fileInfo.Name).ToArray();
            Assert.Contains(_file1.Name, fileNames);
            Assert.Contains(_file2.Name, fileNames);
        }

        public void Dispose()
        {
            if (Directory.Exists(_startingDirectory.FullName))
            {
                Directory.Delete(_startingDirectory.FullName, true);
            }
        }
    }
}
