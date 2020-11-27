using System;
using System.Windows.Forms;

namespace Reading
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);            
            bool createNew;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.Run(new LoginForm());
                }
                else
                {
                    MessageBox.Show("应用程序已经在运行中...");
                    System.Threading.Thread.Sleep(1000);
                    System.Environment.Exit(1);
                }
            }
        }
    }
}
