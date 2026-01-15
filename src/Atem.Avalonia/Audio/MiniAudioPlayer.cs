#region References

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using MiniAudioEx.Core.StandardAPI;
using MiniAudioEx.Native;

#endregion

namespace Atem.Avalonia.Audio;

public partial class MiniAudioPlayer : IDisposable
{
	#region Fields

	private AudioSource _currentSource;
	private MemoryStream _currentStream;
	private readonly ConcurrentQueue<float> _pcmQueue;

	#endregion

	#region Constructors

	public MiniAudioPlayer()
	{
		_pcmQueue = new();

		AudioContext.Initialize(44100, 2);

		var updateThread = new Thread(UpdateLoop) { IsBackground = true };
		updateThread.Start();
	}

	#endregion

	#region Properties

	public bool IsPlaying { get; set; }

	public float Volume
	{
		get => _currentSource?.Volume ?? 0;
		set
		{
			var source = _currentSource;
			if (source != null)
			{
				source.Volume = value;
			}
		}
	}

	#endregion

	#region Methods

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public void Pause()
	{
		_currentSource?.Stop();
		IsPlaying = false;
	}

	public void Play()
	{
		_currentSource?.Play();
		IsPlaying = _currentSource?.IsPlaying ?? false;
	}

	public void Seek(double position)
	{
		// Not supported for procedural streaming sources
		// File-based clips might support it internally, but not exposed here
	}

	public void Speak(string message)
	{
		// Implement Windows SpeechSynthesizer if needed later
	}

	public void StartStreaming()
	{
		Stop();

		_currentSource = new AudioSource();
		_currentSource.Read += OnReadPcm;
		_currentSource.Volume = 1f;
		_currentSource.Play();
		IsPlaying = true;
	}

	public void Stop()
	{
		var source = _currentSource;
		if (source != null)
		{
			source.Read -= OnReadPcm;
		}
		_currentSource?.Stop();
		_currentSource = null;
		IsPlaying = false;
	}

	public void WriteSamples(ReadOnlySpan<short> interleavedStereoPcm16)
	{
		if (interleavedStereoPcm16.IsEmpty)
		{
			return;
		}

		for (var i = 0; i < interleavedStereoPcm16.Length; i++)
		{
			_pcmQueue.Enqueue(interleavedStereoPcm16[i] / 32768f);
		}
	}

	protected void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}
		Stop();
		AudioContext.Deinitialize();
	}

	protected void PlayFile(string audioFilePath)
	{
		Stop();

		var clip = new AudioClip(audioFilePath);
		_currentSource = new AudioSource();
		_currentSource.Play(clip);
		IsPlaying = true;
	}

	protected void PlayStream(Stream wavStream)
	{
		Stop();

		_currentStream = wavStream as MemoryStream ?? new MemoryStream();
		if (_currentStream != wavStream)
		{
			wavStream.Position = 0;
			wavStream.CopyTo(_currentStream);
		}

		_currentStream.Position = 0;
		var data = _currentStream.ToArray();

		var clip = new AudioClip(data);
		_currentSource = new AudioSource();
		_currentSource.Play(clip);
		IsPlaying = true;
	}

	protected void PlayUri(Uri uri)
	{
		Stop();
		PlayFile(uri.LocalPath);
	}

	private void OnReadPcm(NativeArray<float> framesOut, ulong frameCount, int channels)
	{
		var totalFrames = (int) frameCount * channels;

		for (var i = 0; i < totalFrames; i++)
		{
			if (_pcmQueue.TryDequeue(out var sample))
			{
				framesOut[i] = sample;
			}
			else
			{
				framesOut[i] = 0f;
			}
		}
	}

	private void UpdateLoop()
	{
		while (true)
		{
			AudioContext.Update();
			Thread.Sleep(10);
		}
	}

	#endregion
}