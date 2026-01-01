#region References

using Atem.Avalonia.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

#endregion

namespace Atem.Avalonia;

public partial class App : Application
{
	#region Properties

	public GameBoyViewModel ViewModel { get; private set; }

	#endregion

	#region Methods

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		ViewModel = GameBoyViewModel.Start();

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = new MainWindow { DataContext =  ViewModel };
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		{
			singleViewPlatform.MainView = new MainView { DataContext = ViewModel };
		}

		base.OnFrameworkInitializationCompleted();
	}

	#endregion
}