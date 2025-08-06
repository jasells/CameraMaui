using LibVLCSharp.Shared;

namespace Video_Demo;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		Core.Initialize(); // This will initialize the VLC core components

		MainPage = new AppShell();
	}
}

