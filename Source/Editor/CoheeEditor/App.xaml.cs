using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CoheeEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Culture setup
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            ParseCommandLineArguments(e.Args);

            UIs.SplashScreen splashScreen = new UIs.SplashScreen();
            splashScreen.Show();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

        }


        private static void ParseCommandLineArguments(string[] args)
        {
            foreach (string argument in args)
            {
                if (argument == "debug")
                    System.Diagnostics.Debugger.Launch();
            }
        }

    }
}
