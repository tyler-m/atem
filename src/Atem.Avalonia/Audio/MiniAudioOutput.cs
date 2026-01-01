#region References

using System;
using System.Runtime.InteropServices;
using Atem.App.Audio;
using Atem.Core.Audio;

#endregion

namespace Atem.Avalonia.Audio;

public sealed class MiniAudioOutput : ISoundService, IDisposable
{
	#region Fields

	private readonly MiniAudioPlayer _audioPlayer;

	#endregion

	#region Constructors

	public MiniAudioOutput()
	{
		_audioPlayer = new MiniAudioPlayer();
		_audioPlayer.StartStreaming();
	}

	#endregion

	#region Properties

	public float Volume
	{
		get => _audioPlayer.Volume;
		set => _audioPlayer.Volume = value;
	}

	#endregion

	#region Methods

	public void Dispose()
	{
		_audioPlayer?.Stop();
		_audioPlayer?.Dispose();
	}

	public void WriteSamples(ReadOnlySpan<short> interleavedStereoPcm16)
	{
		if (interleavedStereoPcm16.IsEmpty)
		{
			return;
		}

		_audioPlayer.WriteSamples(interleavedStereoPcm16);
	}

	#endregion

	public void Play()
	{
		// MiniAudioPlayer already starts streaming in constructor
		// This method might be used to resume after pause — if you add pause support later
		// For now, it's safe to do nothing or ensure streaming
		_audioPlayer.StartStreaming(); // idempotent if already running
	}

	public void SubmitBuffer(byte[] buffer)
	{
		if (buffer == null || buffer.Length == 0)
			return;

		// Safety check: must be even length for int16 stereo
		if (buffer.Length % 4 != 0)
			throw new ArgumentException("Buffer length must be multiple of 4 bytes (stereo int16).");

		// Interpret byte[] as short[] without copying (zero-allocation when possible)
		ReadOnlySpan<byte> byteSpan = buffer;
		ReadOnlySpan<short> shortSpan = MemoryMarshal.Cast<byte, short>(byteSpan);

		// Ensure correct byte order? Usually little-endian on PC, which matches short layout
		WriteSamples(shortSpan);
	}
}