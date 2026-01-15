#region References

using Atem.App.Graphics;
using Atem.Core;
using Atem.Core.Graphics;

#endregion

namespace Atem.Avalonia.Views;

public class Screen : IScreen
{
	#region Fields

	private readonly Emulator _emulator;
	private readonly Window _window;

	#endregion

	#region Constructors

	public Screen(Emulator emulator, Window window)
	{
		_emulator = emulator;
		_window = window;
	}

	#endregion

	#region Properties

	public int SizeFactor { get; set; }
	public bool SizeLocked { get; set; }

	#endregion
}