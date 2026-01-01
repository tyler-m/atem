#region References

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.Threading;

#endregion

namespace Atem.Avalonia.Controls;

public class GameBoyScreen : Control
{
	#region Fields

	public static readonly DirectProperty<GameBoyScreen, IReadOnlyList<uint>> FrameBufferProperty;

	private readonly WriteableBitmap _bitmap;
	private IReadOnlyList<uint> _frameBuffer;
	private readonly uint[] _localBuffer = new uint[160 * 144];
	private bool _needsUpdate;

	#endregion

	#region Constructors

	public GameBoyScreen()
	{
		_bitmap = new WriteableBitmap(
			new PixelSize(160, 144),
			new Vector(96, 96),
			PixelFormat.Bgra8888,
			AlphaFormat.Premul);

		// Ensure we redraw when size changes
		this.GetObservable(BoundsProperty)
			.Subscribe(new AnonymousObserver<Rect>(_ => InvalidateVisual()));
	}

	static GameBoyScreen()
	{
		FrameBufferProperty = AvaloniaProperty.RegisterDirect<GameBoyScreen, IReadOnlyList<uint>>(
			nameof(FrameBuffer),
			o => o._frameBuffer,
			(o, v) => o.FrameBuffer = v);
	}

	#endregion

	#region Properties

	public IReadOnlyList<uint> FrameBuffer
	{
		get => _frameBuffer;
		set
		{
			SetAndRaise(FrameBufferProperty, ref _frameBuffer, value);
			UpdatePixelBuffer();
		}
	}

	#endregion

	#region Methods

	public void PresentNewFrame()
	{
		UpdatePixelBuffer();
	}

	public override void Render(DrawingContext context)
	{
		if (_bitmap == null)
		{
			return;
		}

		if (_needsUpdate)
		{
			using var bitmapLock = _bitmap.Lock();
			var tempIntView = MemoryMarshal.Cast<uint, int>(_localBuffer).ToArray();

			Marshal.Copy(tempIntView, 0, bitmapLock.Address, tempIntView.Length);

			_needsUpdate = false;
		}

		// Correct way to define source rect: (x, y, width, height)
		var sourceRect = new Rect(0, 0, 160, 144);

		// Preserve aspect ratio
		var viewBounds = Bounds; // Already includes padding/margins
		var scale = Math.Min(viewBounds.Width / 160, viewBounds.Height / 144);
		var scaledWidth = 160 * scale;
		var scaledHeight = 144 * scale;

		var destRect = new Rect(
			viewBounds.Center.X - (scaledWidth / 2),
			viewBounds.Center.Y - (scaledHeight / 2),
			scaledWidth,
			scaledHeight);

		context.DrawImage(_bitmap, sourceRect, destRect);
	}

	private void UpdatePixelBuffer()
	{
		if ((_frameBuffer == null) || (_frameBuffer.Count < (160 * 144)))
		{
			return;
		}

		// Convert ARGB (common in emulators) to BGRA (expected by WriteableBitmap)
		for (var i = 0; i < _localBuffer.Length; i++)
		{
			var argb = _frameBuffer[i];
			var a = (byte) ((argb >> 24) & 0xFF);
			var r = (byte) ((argb >> 16) & 0xFF);
			var g = (byte) ((argb >> 8) & 0xFF);
			var b = (byte) (argb & 0xFF);

			_localBuffer[i] = (uint) ((a << 24) | (r << 16) | (g << 8) | b);
		}

		_needsUpdate = true;

		Dispatcher.UIThread.Post(InvalidateVisual);
	}

	#endregion
}