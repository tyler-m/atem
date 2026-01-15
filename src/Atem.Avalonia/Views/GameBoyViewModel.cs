#region References

using Atem.Avalonia.Audio;
using Atem.Avalonia.Config;
using Atem.Avalonia.Input;
using Atem.Core;
using System;
using System.IO;
using Atem.App.Audio;
using Atem.App.Config;
using Atem.App.Factories;
using Atem.App.Graphics;
using Atem.App.Input;
using Atem.App.Input.Configure;
using Atem.App.IO;
using Atem.App.Saving;
using Atem.App.Shutdown;
using Atem.App.View;

#endregion

namespace Atem.Avalonia.Views;

public class GameBoyViewModel : IAtemView
{
	#region Fields

	private readonly InputManager _inputManager;
	private readonly IShutdownService _shutdownService;
	private readonly ISoundService _soundService;
	private readonly Window _window;

	#endregion

	#region Constructors

	public GameBoyViewModel(Emulator emulator, Window window, ISoundService soundService, InputManager inputManager,
		KeyProvider keyProvider, IShutdownService shutdownService)
	{
		Emulator = emulator;
		KeyProvider = keyProvider;
		_soundService = soundService;
		_shutdownService = shutdownService;
		_inputManager = inputManager;

		_window = window;
		Emulator.Paused = true;
	}

	#endregion

	#region Properties

	public Emulator Emulator { get; }

	public KeyProvider KeyProvider { get; private set; }

	#endregion

	#region Methods

	public void Exit()
	{
	}

	public void Initialize()
	{
		_soundService.Play();
	}

	public void OnExit()
	{
		_shutdownService.Shutdown();
	}

	public static GameBoyViewModel Start()
	{
		Window window = new();

		var emulator = EmulatorFactory.Create();
		var keyProvider = new KeyProvider();

		Screen screen = new(emulator, window);
		SoundService soundService = new(emulator.Audio);
		FileSaveStateService saveStateService = new(emulator);
		FileCartridgeLoader cartridgeLoader = new(emulator);
		FileBatterySaveService batterySaveService = new(emulator);
		InputManager inputManager = new(keyProvider);
		RecentFilesService recentFilesService = new();
		var serialLink = new TCPSerialLink(emulator.Serial);


		AtemConfigDefaultsProvider atemConfigDefaultsProvider = new();
		FileConfigStore<AtemConfig> configStore = new(atemConfigDefaultsProvider, Path.Join(GetApplicationDataLocation(), "config.json"));
		AtemConfigService configService = new(configStore, window, screen, emulator.Audio, inputManager, recentFilesService);
		ShutdownService shutdownService = new(emulator, configService, cartridgeLoader, batterySaveService);

		//ImGuiRenderer imGui = new();
		// ViewUIManager viewUIManager = new(imGui, emulator, saveStateService, batterySaveService, cartridgeLoader, screen, inputManager, recentFilesService, serialLink);

		var view = new GameBoyViewModel(emulator, window, soundService, inputManager, keyProvider, shutdownService);

		//view.OnInitialize += () => imGui.Initialize(view); // link ImGuiRenderer and View instances

		// add commands to the input manager
		AtemCommandConfigurator atemCommandConfigurator = new(emulator);
		ViewCommandConfigurator viewCommandConfigurator = new(view);
		StateCommandConfigurator stateCommandConfigurator = new(saveStateService, cartridgeLoader.Context);
		atemCommandConfigurator.Configure(inputManager);
		viewCommandConfigurator.Configure(inputManager);
		stateCommandConfigurator.Configure(inputManager);

		configService.LoadConfig(); // grab config from file
		configService.LoadValues(); // load values from config

		//view.Run();
		return view;
	}

	protected static string GetApplicationDataLocation()
	{
		var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		//return DevicePlatform == DevicePlatform.Windows

		//	// C:\Users\[User]\AppData\Local + ApplicationName
		//	? Path.Combine(localAppData, ApplicationName)
		//	: localAppData;
		return localAppData;
	}

	public void Update()
	{
		_inputManager.Update();

		Emulator.Update();
	}

	#endregion
}