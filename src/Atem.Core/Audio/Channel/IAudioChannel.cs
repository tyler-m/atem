
namespace Atem.Core.Audio.Channel
{
    public interface IAudioChannel
    {
        public bool UserMute { get; set; }
        public void Clock();
        public byte ProvideSample();
    }
}
