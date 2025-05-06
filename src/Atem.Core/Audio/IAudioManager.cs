using System.Collections.Generic;
using Atem.Core.Audio.Channel;
using Atem.Core.Memory;

namespace Atem.Core.Audio
{
    public interface IAudioManager: IMemoryProvider, IAudioBufferProvider
    {
        public float VolumeFactor { get; set; }
        public IList<IAudioChannel> Channels { get; }
    }
}
