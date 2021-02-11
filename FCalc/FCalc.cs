using System;
using PluginBase;
using System.Runtime.InteropServices;
using System.Data;

namespace FCalc
{
    public class FCalc : IPlugin
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        public string name => "FCalc";

        public string description => "A calculator built into FLauncher";

        public bool CommandEntered(string text_entered, string parameter)
        {
            bool inputHandled = false;
            if (text_entered.ToLower().StartsWith("calc/"))
            {
                var expression = text_entered.Remove(0,5);
                DataTable dataTable = new DataTable();
                try
                {
                    var answer = dataTable.Compute(expression, "");
                    MessageBox((IntPtr)0, answer.ToString(), "FCalc", 0);
                }
                catch(Exception ex)
                {
                    MessageBox((IntPtr)0, ex.Message, "FCalc Exception Encountered", 0);
                }
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
