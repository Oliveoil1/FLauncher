namespace PluginBase
{
	public interface IPlugin
	{
		string name { get; }
		string description { get; }

		bool CommandEntered(string text_entered, string parameter);
		int Init();
	}
}
