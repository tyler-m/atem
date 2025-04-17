using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Atem.IO
{
    /// <summary>
    /// Tracks and stores the most recently accessed files in a bounded list. New
    /// entries are added to the front, duplicates are reordered, and the oldest
    /// files are discarded once the maximum limit is reached. Consumers can
    /// retrieve the list as file paths or as <see cref="FileInfo"/> objects.
    /// </summary>
    public class RecentFilesService : IRecentFilesService
    {
        public const int MaxFiles = 10;

        private readonly List<FileInfo> _recentFiles = [];

        public IEnumerable<string> RecentFiles
        {
            get => _recentFiles.Select(info => info.FullName).ToList();
            set
            {
                _recentFiles.Clear();

                foreach (string filePath in value)
                {
                    _recentFiles.Add(new FileInfo(filePath));
                }
            }
        }

        public IEnumerable<FileInfo> RecentFilesInfo
        {
            get => _recentFiles;
        }

        public void Add(string filePath)
        {
            FileInfo newFileInfo = new(filePath);

            int index = _recentFiles.FindIndex(fi => fi.FullName.Equals(newFileInfo.FullName, StringComparison.OrdinalIgnoreCase));
            if (index != -1)
            {
                // if the file we're adding is already in the recent files list,
                // we remove it and then add it back to the front of the list
                _recentFiles.RemoveAt(index);
            }
            else
            {
                // if we're at the maximum number of files to list, we remove one
                if (_recentFiles.Count == MaxFiles)
                {
                    _recentFiles.RemoveAt(_recentFiles.Count - 1);
                }
            }

            _recentFiles.Insert(0, newFileInfo);
        }
    }
}
