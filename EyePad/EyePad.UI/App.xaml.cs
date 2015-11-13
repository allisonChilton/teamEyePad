using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EyeXFramework.Wpf;

namespace EyePad.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Maintain a reference to the Tobii EyeX Host
        // object in the application so that it isn't
        // garbage collected by the CLR while the app
        // is still running
        private WpfEyeXHost _eyeXHost;

        public App()
        {
            _eyeXHost = new WpfEyeXHost();
            _eyeXHost.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Dispose of the application's WpfEyeXHost
            // object upon exitting the application
            _eyeXHost.Dispose();
        }
    }
}
