namespace Atem.Core.Audio.Channel
{
    public interface IAudioChannel
    {
        public void Clock();
        public byte ProvideSample();
    }
}
