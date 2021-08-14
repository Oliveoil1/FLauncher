using PluginBase;

namespace DemoPlugin
{
	public class DemoPlugin : IPlugin
	{
		public string name => "Demo Plugin";

		public string description => "A demo of FLauncher's plugin system.";

		public bool CommandEntered(string text_entered, string parameter)
		{
			bool inputHandled = false;
			if (text_entered.ToLower() == "demo plugin")
			{
				FLauncher.FlauncherGlobal.ShowMessage("this is a test");
				inputHandled = true;
			}

			return inputHandled;
		}

		public int Init()
		{
			return 0;
		}
	}
}
