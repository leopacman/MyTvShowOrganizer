using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace MyTvShowsOrganizer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
         
           // Application.SetCompatibleTextRenderingDefault(false);

             //_________________________________________
            bool moreThanOneIsRunning = true;
            for (int i = 0; i < 10; i++)
            {
                Process[] processes = Process.GetProcessesByName("MyTvShowsOrganizer");
                if (processes.Length > 1)
                {
                     Thread.Sleep(500); //waiting for possible closing delaying method.
                    //moreThanOneIsRunning = true;
                }
                else
                {
                    moreThanOneIsRunning = false;
                    break;
                }
            }
            if (moreThanOneIsRunning)
            {
                MessageBox.Show("This program is already running. Maybe minimized at Tray. If you've just updated, try again from Desktop Icon.");
                return;
            }
            //_________________________________________________________

            Application.Run(new Form_Main());
        }
    }
}
