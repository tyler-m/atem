using System;
using System.Collections.Generic;
using System.IO;

namespace Atem.IO
{
    /// <summary>
    /// Provides an interface for navigating and browsing a file system.
    /// Supports directory traversal, listing subdirectories and files,
    /// and filtering by extensions.
    /// </summary>
    public class FileBrowser
    {
        private DirectoryInfo _currentDirectory;

        public DirectoryInfo CurrentDirectory => _currentDirectory;

        public FileBrowser(string startingDirectory)
        {
            _currentDirectory = new DirectoryInfo(startingDirectory);
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
                if (extensions == null || extensions.Length == 0)
                {
                    yield return file;
                    continue;
                }

                foreach (string extension in extensions)
                {
                    if (file.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return file;
                        break;
                    }
                }
            }
        }
    }
}