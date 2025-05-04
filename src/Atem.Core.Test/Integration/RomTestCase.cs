
namespace Atem.Core.Test.Integration
{
    public class RomTestCase
    {
        public string RelativeFilePath { get; set; }
        public int MaxUpdates { get; set; }
        public string ExpectedScreenHash { get; set; }

        public RomTestCase(string relativeFilePath, int maxUpdates, string expectedScreenHash)
        {
            RelativeFilePath = relativeFilePath;
            MaxUpdates = maxUpdates;
            ExpectedScreenHash = expectedScreenHash;
        }
    }
}
