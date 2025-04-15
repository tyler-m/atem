using Atem.Config;

namespace Atem.Test.Config
{
    internal class StubAtemConfigDefaultsProvider : IConfigDefaultsProvider<AtemConfig>
    {
        public AtemConfig GetDefaults()
        {
            return new AtemConfig() {
                WindowWidth = int.MaxValue
            };
        }
    }
}
