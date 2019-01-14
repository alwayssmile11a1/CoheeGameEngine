using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cohee.Editor.UIs;

namespace Cohee.Editor
{
    public static class CoheeEditorApp
    {
        public const string EditorLogfilePath = "logfile_editor.txt";
        public const string EditorPrevLogfileName = "logfile_editor_{0}.txt";
        public const string EditorPrevLogfileDir = "Temp";


        private static bool needsRecovery = false;
        private static MainWindow mainWindow = null;
        private static EditorLogOutput memoryLogOutput = null;
        private static bool appSuspended = true;

        public static void Init(MainWindow mainWindow)
        {
            CoheeEditorApp.mainWindow = mainWindow;

            // Set up an in-memory data log so plugins can access the log history when needed
            CoheeEditorApp.memoryLogOutput = new EditorLogOutput();
            Logs.AddGlobalOutput(memoryLogOutput);

            // Create working directories, if not existing yet.
            if (!Directory.Exists(CoheeApp.DataDirectory))
            {
                Directory.CreateDirectory(CoheeApp.DataDirectory);
                using (FileStream s = File.OpenWrite(Path.Combine(CoheeApp.DataDirectory, "WorkingFolderIcon.ico")))
                {
                    Properties.GeneralRes.IconWorkingFolder.Save(s);
                }
                using (StreamWriter w = new StreamWriter(Path.Combine(CoheeApp.DataDirectory, "desktop.ini")))
                {
                    w.WriteLine("[.ShellClassInfo]");
                    w.WriteLine("ConfirmFileOp=0");
                    w.WriteLine("NoSharing=0");
                    w.WriteLine("IconFile=WorkingFolderIcon.ico");
                    w.WriteLine("IconIndex=0");
                    w.WriteLine("InfoTip=This is Dualitors working folder");
                }

                DirectoryInfo dirInfo = new DirectoryInfo(CoheeApp.DataDirectory);
                dirInfo.Attributes |= FileAttributes.System;

                FileInfo fileInfoDesktop = new FileInfo(Path.Combine(CoheeApp.DataDirectory, "desktop.ini"));
                fileInfoDesktop.Attributes |= FileAttributes.Hidden;

                FileInfo fileInfoIcon = new FileInfo(Path.Combine(CoheeApp.DataDirectory, "WorkingFolderIcon.ico"));
                fileInfoIcon.Attributes |= FileAttributes.Hidden;
            }
            if (!Directory.Exists(CoheeApp.PluginDirectory)) Directory.CreateDirectory(CoheeApp.PluginDirectory);
            if (!Directory.Exists(EditorHelper.SourceDirectory)) Directory.CreateDirectory(EditorHelper.SourceDirectory);
            if (!Directory.Exists(EditorHelper.SourceMediaDirectory)) Directory.CreateDirectory(EditorHelper.SourceMediaDirectory);
            if (!Directory.Exists(EditorHelper.SourceCodeDirectory)) Directory.CreateDirectory(EditorHelper.SourceCodeDirectory);

            //// Initialize Package Management system
            //packageManager = new PackageManager();

            // Initialize Cohee Core
            EditorHintImageAttribute.ImageResolvers += EditorHintImageResolver;
            CoheeApp.PluginManager.PluginsReady += DualityApp_PluginsReady;
            CoheeApp.Init(
                CoheeApp.ExecutionEnvironment.Editor,
                CoheeApp.ExecutionContext.Editor,
                new DefaultAssemblyLoader(),
                null);

            // Initialize the plugin manager for the editor. We'll use the same loader as the core.
            pluginManager.Init(DualityApp.PluginManager.AssemblyLoader);

            // Need to load editor plugins before initializing the graphics context, so the backend is available
            pluginManager.LoadPlugins();

            // Need to initialize graphics context and default content before instantiating anything that could require any of them
            InitMainGraphicsContext();
            DualityApp.InitPostWindow();

            LoadUserData();
            pluginManager.InitPlugins();

            // Allow the engine to run
            appSuspended = false;
        }

        public static void Terminate(bool byUser)
        {

        }


        private static void InitMainGraphicsContext()
        {

        }

        private static void LoadUserData()
        {

        }

    }
}
