using System;
using System.Collections.Generic;
using System.Text;

namespace PluginGlue
{
	public abstract class Glue
	{
		public abstract void CreateWindow(List<PluginControl> controls);
	}

	public class PluginControl
	{
		int left = 0;
		int right = 0;
	}

	public class PluginLabel : PluginControl
	{
		string text;
	}
}
