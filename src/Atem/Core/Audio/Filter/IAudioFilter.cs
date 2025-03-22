
namespace Atem.Core.Audio.Filter
{
    internal interface IAudioFilter
    {
        public (float leftChannel, float rightChannel) Filter(float leftChannel, float rightChannel);
    }
}