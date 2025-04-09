
namespace Atem.Core.Audio
{
    public interface IAudioManager: IAudioBufferProvider
    {
        public float VolumeFactor { get; set; }
    }
}
