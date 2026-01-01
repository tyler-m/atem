#region References

using System.Collections.Generic;
using Atem.App.Input;
using Avalonia.Input;

#endregion

namespace Atem.Avalonia.Input;

public class KeyProvider : IKeyProvider
{
	#region Constructors

	public KeyProvider()
	{
		_previous = [];
		_current = [];
	}

	#endregion

	#region Fields

	private readonly HashSet<Key> _current;
	private readonly HashSet<Key> _previous;

	#endregion

	#region Properties

	public bool Alt => IsKeyDown((int)Key.LeftAlt) || IsKeyDown((int)Key.RightAlt);

	public bool Control => IsKeyDown((int)Key.LeftCtrl) || IsKeyDown((int)Key.RightCtrl);

	public IEnumerable<int> PressedKeys
	{
		get
		{
			foreach (var key in _current)
				if (!_previous.Contains(key))
					yield return (int)key;
		}
	}

	public bool Shift => IsKeyDown((int)Key.LeftShift) || IsKeyDown((int)Key.RightShift);

	#endregion

	#region Methods

	public bool DidKeyChange(int keyCode)
	{
		var key = (Key)keyCode;
		var changed = _current.Contains(key) != _previous.Contains(key);
		return changed;
	}

	public string GetKeyString(int keyCode)
	{
		return ((Key)keyCode).ToString();
	}

	public bool IsActive(Keybind keybind)
	{
		if (keybind.Shift && !Shift) return false;
		if (keybind.Control && !Control) return false;
		if (keybind.Alt && !Alt) return false;

		return DidKeyChange(keybind.Key);
	}

	public bool IsKeyDown(int keyCode)
	{
		return _current.Contains((Key)keyCode);
	}

	public bool IsModifier(int keyCode)
	{
		var key = (Key)keyCode;
		return key is Key.LeftShift or Key.RightShift or
			Key.LeftCtrl or Key.RightCtrl or
			Key.LeftAlt or Key.RightAlt;
	}

	public void PostUpdate()
	{
		_previous.Clear();
		foreach (var key in _current) _previous.Add(key);
	}

	public void PreUpdate()
	{
	}

	internal void HandleKeyDown(KeyEventArgs e)
	{
		_current.Add(e.Key);
	}

	internal void HandleKeyUp(KeyEventArgs e)
	{
		_current.Remove(e.Key);
	}

	private bool WasKeyUp(int keyCode)
	{
		return !_previous.Contains((Key)keyCode);
	}

	#endregion
}