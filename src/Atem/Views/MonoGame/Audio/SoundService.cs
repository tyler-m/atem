using Microsoft.Xna.Framework.Audio;
using Atem.Core.Audio;
using Atem.Views.Audio;

namespace Atem.Views.MonoGame.Audio
{
    public class SoundService : ISoundService
    {
        private readonly DynamicSoundEffectInstance _instance;

        public SoundService(IAudioBufferProvider audioBufferProvider)
        {
            _instance = new DynamicSoundEffectInstance(AudioManager.SAMPLE_RATE, AudioChannels.Stereo);
            audioBufferProvider.OnFullAudioBuffer += SubmitBuffer;
        }

        public void Play()
        {
            _instance.Play();
        }

        public void SubmitBuffer(byte[] buffer)
        {
            if (_instance.PendingBufferCount <= 10)
            {
                _instance.SubmitBuffer(buffer);
            }
        }
    }
}
