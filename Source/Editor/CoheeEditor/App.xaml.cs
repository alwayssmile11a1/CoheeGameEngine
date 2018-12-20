using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Cohee.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private StreamWriter logfileWriter;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Culture setup
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            //Use in case we want to debug deloyed application
            ParseCommandLineArguments(e.Args);

            //Setup logfile
            ArchiveOldLogfile();
            CreateLogfile();

            //Catch unhandled exception of entire app
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            //Run SplashScreen
            UIs.SplashScreen splashScreen = new UIs.SplashScreen();
            splashScreen.Show();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            CloseLogfile();
        }


        private void ParseCommandLineArguments(string[] args)
        {
            foreach (string argument in args)
            {
                if (argument == "debug")
                    System.Diagnostics.Debugger.Launch();
            }
        }


        private void ArchiveOldLogfile()
        {
            try
            {
                // If there is an existing logfile, archive it for diagnostic purposes
                FileInfo prevLogfile = new FileInfo(CoheeEditorApp.EditorLogfilePath);
                if (prevLogfile.Exists)
                {
                    if (!Directory.Exists(CoheeEditorApp.EditorPrevLogfileDir))
                        Directory.CreateDirectory(CoheeEditorApp.EditorPrevLogfileDir);

                    string timestampToken = prevLogfile.LastWriteTimeUtc.ToString("yyyy-MM-dd-T-HH-mm-ss");
                    string prevLogfileName = string.Format(CoheeEditorApp.EditorPrevLogfileName, timestampToken);
                    string prevLogFilePath = Path.Combine(CoheeEditorApp.EditorPrevLogfileDir, prevLogfileName);

                    prevLogfile.MoveTo(prevLogFilePath);
                }
            }
            catch (Exception e)
            {
                //Logs.Core.WriteWarning("Unable to archive old logfile: {0}", LogFormat.Exception(e));
            }
        }
        private void CreateLogfile()
        {
            //if (logfileOutput != null || logfileWriter != null)
            //    CloseLogfile();

            //try
            //{
            //    logfileWriter = new StreamWriter(CoheeEditorApp.EditorLogfilePath);
            //    logfileWriter.AutoFlush = true;
            //    logfileOutput = new TextWriterLogOutput(logfileWriter);
            //    Logs.AddGlobalOutput(logfileOutput);
            //}
            //catch (Exception e)
            //{
            //    Logs.Core.WriteWarning("Unable to create logfile: {0}", LogFormat.Exception(e));
            //}
        }
        private void CloseLogfile()
        {
            //if (logfileOutput != null)
            //{
            //    Logs.RemoveGlobalOutput(logfileOutput);
            //    logfileOutput = null;
            //}
            //if (logfileWriter != null)
            //{
            //    logfileWriter.Flush();
            //    logfileWriter.Close();
            //    logfileWriter = null;
            //}
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //try
            //{
            //    Logs.Editor.WriteError(LogFormat.Exception(e.Exception));
            //}
            //catch (Exception) { /* Ensure we're not causing any further exception by logging... */ }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //try
            //{
            //    Logs.Editor.WriteError(LogFormat.Exception(e.ExceptionObject as Exception));
            //}
            //catch (Exception) { /* Ensure we're not causing any further exception by logging... */ }
        }
    }
}
