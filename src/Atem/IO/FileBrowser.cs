using System;
using System.Collections.Generic;
using System.IO;

namespace Atem.IO
{
    internal class FileBrowser
    {
        private readonly DirectoryInfo _rootDirectory;
        private DirectoryInfo _currentDirectory;

        public DirectoryInfo CurrentDirectory { get => _currentDirectory; }

        public FileBrowser(string rootPath)
        {
            _rootDirectory = new DirectoryInfo(rootPath);
            _currentDirectory = new DirectoryInfo(_rootDirectory.FullName);
        }

        public void NavigateTo(DirectoryInfo directory)
        {
            _currentDirectory = directory;
        }

        public void NavigateUp()
        {
            if (_currentDirectory.Parent != null)
            {
                _currentDirectory = _currentDirectory.Parent;
            }
        }

        public IEnumerable<DirectoryInfo> GetDirectories()
        {
            return _currentDirectory.EnumerateDirectories();
        }

        public IEnumerable<FileInfo> GetFiles(string[] extensions = null)
        {
            foreach (FileInfo file in _currentDirectory.EnumerateFiles())
            {
                foreach (string extension in extensions)
                {
                    if (file.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return file;
                    }
                }
            }
        }
    }
}