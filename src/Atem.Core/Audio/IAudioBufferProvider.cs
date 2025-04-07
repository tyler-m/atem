using System;

namespace Atem.Core.Audio
{
    public interface IAudioBufferProvider
    {
        public event Action<byte[]> OnFullAudioBuffer;
    }
}
