
namespace Atem.Core.Test.Integration
{
    public static class RomPathResolver
    {
        private static readonly string _rootAnchorFileName = "Atem.sln";
        private static string? _cachedRootDirectory;

        public static string Resolve(string relativeFilePath)
        {
            string rootDirectory = FindProjectRoot();
            string fullPath = Path.Combine(rootDirectory, relativeFilePath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            string baseDirectoryPath = Path.Combine(AppContext.BaseDirectory, relativeFilePath);
            if (File.Exists(baseDirectoryPath))
            {
                return baseDirectoryPath;
            }

            throw new FileNotFoundException($"ROM not found: {relativeFilePath}");
        }

        private static string FindProjectRoot()
        {
            if (!string.IsNullOrEmpty(_cachedRootDirectory))
            {
                return _cachedRootDirectory;
            }

            string? directory = AppContext.BaseDirectory;

            while (!string.IsNullOrEmpty(directory))
            {
                if (File.Exists(Path.Combine(directory, _rootAnchorFileName)))
                {
                    _cachedRootDirectory = directory;
                    return directory;
                }

                directory = Directory.GetParent(directory)?.FullName;
            }

            throw new DirectoryNotFoundException("Could not locate project root.");
        }
    }
}
