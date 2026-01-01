#region References

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

#endregion

namespace Atem.Avalonia.Views;

public partial class MainView : UserControl
{
	#region Constants

	/// <summary>
	/// Exact GB frame rate (~16.742 ms)
	/// </summary>
	private const double TargetFrameTimeMs = 1000.0 / 59.72750056960506;

	#endregion

	#region Fields

	private double _accumulator;
	private Action<TimeSpan> _animationCallback;
	private readonly Stopwatch _frameTimer = Stopwatch.StartNew();

	#endregion

	#region Constructors

	public MainView()
	{
		InitializeComponent();
		Focusable = true;
	}

	#endregion

	#region Properties

	public GameBoyViewModel ViewModel => (GameBoyViewModel) DataContext!;

	#endregion

	#region Methods

	/// <summary>
	/// Read the embedded binary file from the assembly.
	/// </summary>
	/// <param name="rom"> The path of the resource to read. </param>
	/// <returns> The value that was read. </returns>
	public static byte[] ReadRom(string rom)
	{
		var path = $"Atem.Avalonia.Roms.{rom}";
		using var stream = typeof(App).Assembly.GetManifestResourceStream(path);

		if (stream == null)
		{
			throw new Exception("Embedded file not found.");
		}

		if (stream == null)
		{
			throw new ArgumentException("Resource not found, stream is empty", nameof(stream));
		}

		var buffer = new byte[16 * 1024];
		using var ms = new MemoryStream();
		int read;

		while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
		{
			ms.Write(buffer, 0, read);
		}

		return ms.ToArray();
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);

		if (Design.IsDesignMode)
		{
			return;
		}

		var screenRenderer = ViewModel.Emulator.Graphics.ScreenRenderer;

		// Connect frame buffer
		GameScreen.FrameBuffer = screenRenderer.ArgbFrameBuffer;

		// New frame ready → update display on UI thread
		screenRenderer.OnFrameReady += OnScreenRendererOnOnFrameReady;

		// Load your test ROM (or move this to a button/menu later)
		//var bytes = File.ReadAllBytes(@"C:\Data\Emulators\GB-GBC\1942 (USA).gbc");
		//var bytes = File.ReadAllBytes(@"C:\Data\Emulators\GB-GBC\Super Mario Bros. Deluxe (USA) (Rev-B).gbc");
		//var bytes = ReadRom("1942 (USA).gbc");
		var bytes = ReadRom("Adventure Island (USA).gb");
		ViewModel.Emulator.LoadCartridge(bytes);
		ViewModel.Emulator.Paused = false;
		ViewModel.Initialize();

		// Start precision game loop
		StartGameLoop();
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		// Clean up to stop the loop when view is removed
		_animationCallback = null;

		var screenRenderer = ViewModel.Emulator.Graphics.ScreenRenderer;
		screenRenderer.OnFrameReady -= OnScreenRendererOnOnFrameReady;

		base.OnDetachedFromVisualTree(e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		ViewModel.KeyProvider.HandleKeyDown(e);
		e.Handled = true;
		base.OnKeyDown(e);
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		ViewModel.KeyProvider.HandleKeyUp(e);
		e.Handled = true;
		base.OnKeyUp(e);
	}

	private void OnScreenRendererOnOnFrameReady()
	{
		GameScreen.PresentNewFrame();
	}

	private void StartGameLoop()
	{
		_animationCallback = _ =>
		{
			// Stop if view is detached or emulator paused
			if ((_animationCallback == null) || ViewModel.Emulator.Paused)
			{
				// Still request next frame to keep UI responsive
				TopLevel.GetTopLevel(this)?.RequestAnimationFrame(_animationCallback);
				return;
			}

			// Accumulate real elapsed time
			_accumulator += _frameTimer.Elapsed.TotalMilliseconds;
			_frameTimer.Restart();

			// Run exactly the needed number of emulation frames
			while (_accumulator >= TargetFrameTimeMs)
			{
				ViewModel.Update();
				_accumulator -= TargetFrameTimeMs;
			}

			// Request next render frame
			TopLevel.GetTopLevel(this)?.RequestAnimationFrame(_animationCallback);
		};

		// Kick off the loop
		TopLevel.GetTopLevel(this)?.RequestAnimationFrame(_animationCallback);
	}


	#endregion
}