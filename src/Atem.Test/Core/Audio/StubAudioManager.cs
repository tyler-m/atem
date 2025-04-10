using Atem.Core.Audio;

namespace Atem.Test.Core.Audio
{
    internal class StubAudioManager : IAudioManager
    {
        public float VolumeFactor { get; set; }

        public event Action<byte[]>? OnFullAudioBuffer;
    }
}
