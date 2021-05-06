using PluginBase;
using System;
using System.Runtime.InteropServices;

namespace DemoPlugin
{
    public class DemoPlugin : IPlugin
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        public string name => "Demo Plugin";

        public string description => "A demo of FLauncher's plugin system.";

        public bool CommandEntered(string text_entered, string parameter)
        {
            bool inputHandled = false;
            if (text_entered.ToLower() == "demo plugin")
            {
                MessageBox((IntPtr)0, "This is a demo plugin", "Demo Plugin", 0);
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
