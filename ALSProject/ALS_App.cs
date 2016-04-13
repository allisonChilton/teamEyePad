using System;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace ALSProject
{
    static class ALS_App
    {
        private static Form mainMenu = null;

        //main loop
        [STAThread]
        static void Main(String[] args)
        {
            //UnsafeFail(); //for testing
            SafeFail(); //for release
        }

        static void SafeFail()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                mainMenu = new MainMenu();
                Application.Run(mainMenu);
            }
            catch (Exception)
            {
                while (RestartProgram()) ;
            }
        }

        static void UnsafeFail()
        {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                mainMenu = new MainMenu();
                Application.Run(mainMenu);
            }
            catch(Exception e)
            {
                LogCrash(e);
                CVInterface.PleaseStop();
                Application.Exit();
                
            }
        }

        public static void LogCrash(Exception e)
        {

            if (!File.Exists("CrashLog.txt"))
            {
                
                StreamWriter filestream = new StreamWriter(File.Create("CrashLog.txt"));
                filestream.Close();
            }

            StreamWriter file = new StreamWriter(File.Open("CrashLog.txt", FileMode.Append));

            file.WriteLine(DateTime.Now.ToLocalTime());
            file.WriteLine(e.ToString());
            file.WriteLine("\n==============================\n");

            file.Close();
        }



        private static bool RestartProgram()
        {
            bool restartApplication = false;
            try
            {
                // Shut down the current app instance.
                Application.Exit();

                // Restart the app passing "/restart [processId]" as cmd line args
                if (mainMenu != null)
                {
                    mainMenu = new MainMenu();
                    Application.Run(mainMenu);
                }
            }
            catch (Exception)
            {
                restartApplication = true;
            }
            return restartApplication;
        }
    }
}
