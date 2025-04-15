using System.Collections.Generic;
using Atem.Core.Audio.Channel;

namespace Atem.Core.Audio
{
    public interface IAudioManager: IAudioBufferProvider
    {
        public float VolumeFactor { get; set; }
        public IList<IAudioChannel> Channels { get; }
    }
}
