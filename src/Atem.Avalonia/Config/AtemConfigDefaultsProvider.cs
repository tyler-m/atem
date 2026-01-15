#region References

using System.Collections.Generic;
using Atem.App.Config;
using Atem.App.Input;
using Atem.App.Input.Command;
using Avalonia.Input;

#endregion

namespace Atem.Avalonia.Config;

public class AtemConfigDefaultsProvider : IConfigDefaultsProvider<AtemConfig>
{
	#region Methods

	public AtemConfig GetDefaults()
	{
		return new AtemConfig
		{
			WindowWidth = 640,
			WindowHeight = 480,
			ScreenSizeFactor = 2,
			ScreenSizeLocked = true,
			Keybinds = GetKeybinds(),
			UserVolumeFactor = 1.0f,
			RecentFiles = []
		};
	}

	private static void AddEmulatorControlCommands(Dictionary<CommandType, List<Keybind>> keybinds)
	{
		keybinds.Add(CommandType.Continue, [new Keybind { CommandType = CommandType.Continue, Key = (int) Key.F5 }]);
		keybinds.Add(CommandType.Pause, [new Keybind { CommandType = CommandType.Pause, Key = (int) Key.Space }]);
	}

	private static void AddGameboyCommands(Dictionary<CommandType, List<Keybind>> keybinds)
	{
		keybinds.Add(CommandType.Up, [new Keybind { CommandType = CommandType.Up, Key = (int) Key.Up }]);
		keybinds.Add(CommandType.Down, [new Keybind { CommandType = CommandType.Down, Key = (int) Key.Down }]);
		keybinds.Add(CommandType.Left, [new Keybind { CommandType = CommandType.Left, Key = (int) Key.Left }]);
		keybinds.Add(CommandType.Right, [new Keybind { CommandType = CommandType.Right, Key = (int) Key.Right }]);
		keybinds.Add(CommandType.B, [new Keybind { CommandType = CommandType.B, Key = (int) Key.Z }]);
		keybinds.Add(CommandType.A, [new Keybind { CommandType = CommandType.A, Key = (int) Key.X }]);
		keybinds.Add(CommandType.Start, [new Keybind { CommandType = CommandType.Start, Key = (int) Key.Enter }]);
		keybinds.Add(CommandType.Select, [new Keybind { CommandType = CommandType.Select, Key = (int) Key.Back }]);
	}

	private static void AddSaveStateCommands(Dictionary<CommandType, List<Keybind>> keybinds)
	{
		keybinds.Add(CommandType.SaveState0, [new Keybind { CommandType = CommandType.SaveState0, Alt = true, Key = (int) Key.D1 }]);
		keybinds.Add(CommandType.SaveState1, [new Keybind { CommandType = CommandType.SaveState1, Alt = true, Key = (int) Key.D2 }]);
		keybinds.Add(CommandType.SaveState2, [new Keybind { CommandType = CommandType.SaveState2, Alt = true, Key = (int) Key.D3 }]);
		keybinds.Add(CommandType.SaveState3, [new Keybind { CommandType = CommandType.SaveState3, Alt = true, Key = (int) Key.D4 }]);
		keybinds.Add(CommandType.SaveState4, [new Keybind { CommandType = CommandType.SaveState4, Alt = true, Key = (int) Key.D5 }]);
		keybinds.Add(CommandType.SaveState5, [new Keybind { CommandType = CommandType.SaveState5, Alt = true, Key = (int) Key.D6 }]);
		keybinds.Add(CommandType.SaveState6, [new Keybind { CommandType = CommandType.SaveState6, Alt = true, Key = (int) Key.D7 }]);
		keybinds.Add(CommandType.SaveState7, [new Keybind { CommandType = CommandType.SaveState7, Alt = true, Key = (int) Key.D8 }]);
		keybinds.Add(CommandType.SaveState8, [new Keybind { CommandType = CommandType.SaveState8, Alt = true, Key = (int) Key.D9 }]);
		keybinds.Add(CommandType.SaveState9, [new Keybind { CommandType = CommandType.SaveState9, Alt = true, Key = (int) Key.D0 }]);

		keybinds.Add(CommandType.LoadState0, [new Keybind { CommandType = CommandType.LoadState0, Control = true, Key = (int) Key.D1 }]);
		keybinds.Add(CommandType.LoadState1, [new Keybind { CommandType = CommandType.LoadState1, Control = true, Key = (int) Key.D2 }]);
		keybinds.Add(CommandType.LoadState2, [new Keybind { CommandType = CommandType.LoadState2, Control = true, Key = (int) Key.D3 }]);
		keybinds.Add(CommandType.LoadState3, [new Keybind { CommandType = CommandType.LoadState3, Control = true, Key = (int) Key.D4 }]);
		keybinds.Add(CommandType.LoadState4, [new Keybind { CommandType = CommandType.LoadState4, Control = true, Key = (int) Key.D5 }]);
		keybinds.Add(CommandType.LoadState5, [new Keybind { CommandType = CommandType.LoadState5, Control = true, Key = (int) Key.D6 }]);
		keybinds.Add(CommandType.LoadState6, [new Keybind { CommandType = CommandType.LoadState6, Control = true, Key = (int) Key.D7 }]);
		keybinds.Add(CommandType.LoadState7, [new Keybind { CommandType = CommandType.LoadState7, Control = true, Key = (int) Key.D8 }]);
		keybinds.Add(CommandType.LoadState8, [new Keybind { CommandType = CommandType.LoadState8, Control = true, Key = (int) Key.D9 }]);
		keybinds.Add(CommandType.LoadState9, [new Keybind { CommandType = CommandType.LoadState9, Control = true, Key = (int) Key.D0 }]);
	}

	private static void AddViewCommands(Dictionary<CommandType, List<Keybind>> keybinds)
	{
		keybinds.Add(CommandType.Exit, [new Keybind { CommandType = CommandType.Exit, Key = (int) Key.Escape }]);
	}

	private Dictionary<CommandType, List<Keybind>> GetKeybinds()
	{
		Dictionary<CommandType, List<Keybind>> keybinds = [];
		AddGameboyCommands(keybinds);
		AddEmulatorControlCommands(keybinds);
		AddSaveStateCommands(keybinds);
		AddViewCommands(keybinds);
		return keybinds;
	}

	#endregion
}