#region References

using Atem.App.Audio;
using Atem.Core.Audio;

#endregion

namespace Atem.Avalonia.Audio;

public class SoundService : ISoundService
{
	#region Fields

	private readonly MiniAudioOutput _output;

	#endregion

	#region Constructors

	public SoundService(IAudioBufferProvider audioBufferProvider)
	{
		_output = new MiniAudioOutput();
		audioBufferProvider.OnFullAudioBuffer += SubmitBuffer;
	}

	#endregion

	#region Methods

	public void Play()
	{
		_output.Play();
	}

	public void SubmitBuffer(byte[] buffer)
	{
		_output.SubmitBuffer(buffer);
	}

	#endregion
}