using Atem.Core.Audio;
using Atem.Core.Audio.Channel;

namespace Atem.Test.Core.Audio
{
    internal class StubAudioManager : IAudioManager
    {
        public float VolumeFactor { get; set; }

        public IList<IAudioChannel> Channels => throw new NotImplementedException();

        public event Action<byte[]>? OnFullAudioBuffer;
    }
}
