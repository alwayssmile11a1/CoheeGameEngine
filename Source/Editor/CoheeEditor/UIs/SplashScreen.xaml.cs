using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cohee.Editor.UIs
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public SplashScreen()
        {
            InitializeComponent();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.worker.RunWorkerAsync(this);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SplashScreen screen = e.Argument as SplashScreen;

            object result = screen.Dispatcher.Invoke(new Func<MainWindow>(screen.InitEditor));
            e.Result = result;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Hide();
            try
            {
                MainWindow mainWindow = e.Result as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Show();
                }
                else
                {
                    // If we don't get a main form back, something went wrong.
                    // Exit, so we don't end up as a hidden process without UI.
                    Application.Current.Shutdown();
                }
            }
            finally
            {
                this.Close();
            }
        }

        protected MainWindow InitEditor()
        {
            try
            {
                MainWindow main = new MainWindow();
                CoheeEditorApp.Init(main);
                return main;
            }
            catch (Exception e)
            {
                Logs.Editor.Write("An error occurred while initializing the editor: {0}", LogFormat.Exception(e));
                return null;
            }
        }

    }
}
