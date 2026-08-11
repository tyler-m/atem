using Microsoft.Xna.Framework.Audio;
using Atem.Core.Audio;
using Atem.Audio;

namespace Atem.Views.MonoGame.Audio;

public class SoundService : ISoundService
{
    private readonly DynamicSoundEffectInstance _instance;
    private readonly AudioBufferPacer _pacer;

    public SoundService(IAudioBufferProvider audioBufferProvider) : this(audioBufferProvider, new AudioBufferPacer())
    {
    }

    public SoundService(IAudioBufferProvider audioBufferProvider, AudioBufferPacer pacer)
    {
        _pacer = pacer;
        _instance = new DynamicSoundEffectInstance(AudioManager.SAMPLE_RATE, AudioChannels.Stereo);
        audioBufferProvider.OnFullAudioBuffer += SubmitBuffer;
    }

    public void Play()
    {
        byte[] silence = new byte[AudioManager.BUFFER_SIZE];

        for (int i = 0; i < _pacer.TargetBuffers; i++)
        {
            _instance.SubmitBuffer(silence);
        }

        _instance.Play();
    }

    public void SubmitBuffer(byte[] buffer)
    {
        if (_instance.State != SoundState.Playing)
        {
            _instance.Play();
        }

        int submitCount = _pacer.GetSubmitCount(_instance.PendingBufferCount);

        for (int i = 0; i < submitCount; i++)
        {
            _instance.SubmitBuffer(buffer);
        }
    }
}
