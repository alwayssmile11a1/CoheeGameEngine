using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Cohee.Backend;
using Cohee.Editor.UIs;

namespace Cohee.Editor
{
    public static class CoheeEditorApp
    {
        public const string EditorLogfilePath = "logfile_editor.txt";
        public const string EditorPrevLogfileName = "logfile_editor_{0}.txt";
        public const string EditorPrevLogfileDir = "Temp";
        public const string UserDataFile = "EditorUserData.xml";
        private const string UserDataDockSeparator = "<!-- DockPanel Data -->";

        private static EditorPluginManager editorPluginManager = new EditorPluginManager();
        private static bool needsRecovery = false;
        private static MainWindow mainWindow = null;
        private static EditorLogOutput editorLogOutput = null;
        private static bool appSuspended = true;

        public static event EventHandler Terminating = null;

        public static EditorPluginManager EditorPluginManager
        {
            get { return editorPluginManager; }
        }

        public static EditorLogOutput EditorLogOutput
        {
            get { return editorLogOutput; }
        }

        public static MainWindow MainWindow
        {
            get { return mainWindow; }
        }

        public static void Init(MainWindow mainWindow)
        {
            CoheeEditorApp.mainWindow = mainWindow;

            // Set up an in-memory data log so plugins can access the log history when needed
            editorLogOutput = new EditorLogOutput();
            Logs.AddGlobalOutput(editorLogOutput);

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
            CoheeApp.CorePluginManager.PluginsReady += CoheeApp_CorePluginsReady;
            CoheeApp.Init(CoheeApp.ExecutionEnvironment.Editor, CoheeApp.ExecutionContext.Editor, new DefaultAssemblyLoader(), null);

            // Initialize the plugin manager for the editor. We'll use the same loader as the core.
            editorPluginManager.Init(CoheeApp.CorePluginManager.AssemblyLoader);
            // Need to load editor plugins before initializing the graphics context, so the backend is available
            editorPluginManager.LoadPlugins();

            // Need to initialize graphics context and default content before instantiating anything that could require any of them
            InitMainGraphicsContext();
            CoheeApp.InitPostWindow();

            //LoadUserData();

            editorPluginManager.InitPlugins();




            // Allow the engine to run
            appSuspended = false;
        }

        public static bool Terminate(bool byUser)
        {
            bool cancel = false;

            //// Display safety message boxes if the close operation is triggered by the user.
            //if (byUser)
            //{
            //    var unsavedResTemp = CoheeEditorApp.UnsavedResources.ToArray();
            //    if (unsavedResTemp.Any())
            //    {
            //        string unsavedResText = unsavedResTemp.Take(5).ToString(r => r.GetType().GetTypeCSCodeName(true) + ":\t" + r.FullName, "\n");
            //        if (unsavedResTemp.Count() > 5)
            //            unsavedResText += "\n" + string.Format(Properties.GeneralRes.Msg_ConfirmQuitUnsaved_Desc_More, unsavedResTemp.Count() - 5);
            //        MessageBoxResult result = MessageBox.Show(
            //            string.Format(Properties.GeneralRes.Msg_ConfirmQuitUnsaved_Desc, "\n\n" + unsavedResText + "\n\n"),
            //            Properties.GeneralRes.Msg_ConfirmQuitUnsaved_Caption,
            //            MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
            //        if (result == MessageBoxResult.Yes)
            //        {
            //            Sandbox.Stop();
            //            CoheeEditorApp.SaveAllProjectData();
            //        }
            //        else if (result == MessageBoxResult.Cancel)
            //            cancel = true;
            //    }
            //}

            // Did we cancel it? Return false.
            if (cancel)
                return false;

            // Otherwise, actually start terminating.
            // From this point on, there's no return - need to re-init the editor afterwards.
            if (Terminating != null)
                Terminating(null, EventArgs.Empty);

            // Unregister events
            EditorHintImageAttribute.ImageResolvers -= EditorHintImageResolver;
            CoheeApp.CorePluginManager.PluginsReady -= CoheeApp_CorePluginsReady;
            //mainForm.Activated -= mainForm_Activated;
            //mainForm.Deactivate -= mainForm_Deactivate;
            //Scene.Leaving -= Scene_Leaving;
            //Scene.Entered -= Scene_Entered;
            //Application.Idle -= Application_Idle;
            //Resource.ResourceSaved -= Resource_ResourceSaved;
            //Resource.ResourceSaving -= Resource_ResourceSaving;
            //Resource.ResourceDisposing -= Resource_ResourceDisposing;
            //FileEventManager.PluginsChanged -= FileEventManager_PluginsChanged;
            //editorObjects.GameObjectsAdded -= editorObjects_GameObjectsAdded;
            //editorObjects.GameObjectsRemoved -= editorObjects_GameObjectsRemoved;
            //editorObjects.ComponentAdded -= editorObjects_ComponentAdded;
            //editorObjects.ComponentRemoving -= editorObjects_ComponentRemoved;

            //// Terminate editor actions
            //editorActions.Clear();

            //// Terminate secondary editor components
            //UndoRedoManager.Terminate();
            //FileEventManager.Terminate();
            //HelpSystem.Terminate();
            //Sandbox.Terminate();
            //PreviewProvider.Terminate();
            //ConvertOperation.Terminate();
            //AssetManager.Terminate();
            //DesignTimeObjectData.Terminate();

            //// Shut down the editor backend
            //CoheeApp.ShutdownBackend(ref graphicsBack);

            // Shut down the plugin manager 
            editorPluginManager.Terminate();

            // Terminate Cohee
            CoheeApp.Terminate();

            // Remove the global in-memory log
            if (editorLogOutput != null)
            {
                Logs.RemoveGlobalOutput(editorLogOutput);
                editorLogOutput = null;
            }

            return true;
        }


        //private static void InitMainGraphicsContext()
        //{
        //    if (mainGraphicsContext != null) return;

        //    if (graphicsBackend == null)
        //        CoheeApp.InitBackend(out graphicsBackend, GetAvailCoheeEditorTypes);

        //    Logs.Editor.Write("Creating editor graphics context...");
        //    Logs.Editor.PushIndent();
        //    try
        //    {
        //        // Currently bound to game-specific settings. Should be decoupled
        //        // from them at some point, so the editor can use independent settings.
        //        mainGraphicsContext = graphicsBackend.CreateContext(
        //            CoheeApp.AppData.MultisampleBackBuffer ?
        //            CoheeApp.UserData.AntialiasingQuality :
        //            AAQuality.Off);
        //    }
        //    catch (Exception e)
        //    {
        //        mainGraphicsContext = null;
        //        Logs.Editor.WriteError("Can't create editor graphics context, because an error occurred: {0}", LogFormat.Exception(e));
        //    }
        //    Logs.Editor.PopIndent();
        //}

        //private static void LoadUserData()
        //{
        //    if (!File.Exists(UserDataFile))
        //    {
        //        File.WriteAllText(UserDataFile, Properties.GeneralRes.DefaultEditorUserData);
        //        if (!File.Exists(UserDataFile)) return;
        //    }

        //    Logs.Editor.Write("Loading user data...");
        //    Logs.Editor.PushIndent();

        //    Encoding encoding = Encoding.Default;
        //    StringBuilder editorData = new StringBuilder();
        //    StringBuilder dockPanelData = new StringBuilder();
        //    using (StreamReader reader = new StreamReader(UserDataFile))
        //    {
        //        encoding = reader.CurrentEncoding;
        //        string line;

        //        // Retrieve pre-DockPanel section
        //        while ((line = reader.ReadLine()) != null && line.Trim() != UserDataDockSeparator)
        //            editorData.AppendLine(line);

        //        // Retrieve DockPanel section
        //        while ((line = reader.ReadLine()) != null)
        //            dockPanelData.AppendLine(line);
        //    }

        //}

        public static IEnumerable<Assembly> GetCoheeEditorAssemblies()
        {
            return editorPluginManager.GetAssemblies();
        }

        public static IEnumerable<TypeInfo> GetAvailCoheeEditorTypes(Type baseType)
        {
            return editorPluginManager.GetTypes(baseType);
        }

        private static object EditorHintImageResolver(string manifestResourceName)
        {
            Assembly[] allAssemblies = CoheeApp.GetCoheeAssemblies().Concat(GetCoheeEditorAssemblies()).Distinct().ToArray();
            foreach (Assembly assembly in allAssemblies)
            {
                string[] resourceNames = assembly.GetManifestResourceNames();
                if (resourceNames.Contains(manifestResourceName))
                {
                    // Since images require to keep their origin stream open, we'll need to copy it to gain independence.
                    using (Stream stream = assembly.GetManifestResourceStream(manifestResourceName))
                    using (Bitmap bitmap = Bitmap.FromStream(stream) as Bitmap)
                    {
                        Bitmap independentBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                        independentBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
                        using (Graphics graphics = Graphics.FromImage(independentBitmap))
                        {
                            graphics.DrawImageUnscaled(bitmap, 0, 0);
                        }
                        return independentBitmap;
                    }
                }
            }
            return null;
        }

        private static void CoheeApp_CorePluginsReady(object sender, CoheePluginEventArgs e)
        {
            foreach (CorePlugin plugin in e.Plugins)
            {
                AnalyzeCorePlugin(plugin);
            }
        }

        public static void AnalyzeCorePlugin(CorePlugin plugin)
        {
            Logs.Editor.Write("Analyzing Core Plugin: {0}", plugin.AssemblyName);
            Logs.Editor.PushIndent();

            // Query references to other Assemblies
            var asmRefQuery = from AssemblyName a in plugin.PluginAssembly.GetReferencedAssemblies()
                              select a.GetShortAssemblyName();
            string thisAsmName = typeof(CoheeEditorApp).Assembly.GetShortAssemblyName();
            foreach (string asmName in asmRefQuery)
            {
                bool illegalRef = false;

                // Scan for illegally referenced Assemblies
                if (asmName == thisAsmName)
                    illegalRef = true;
                else if (editorPluginManager.LoadedPlugins.Any(p => p.PluginAssembly.GetShortAssemblyName() == asmName))
                    illegalRef = true;

                // Warn about them
                if (illegalRef)
                {
                    Logs.Editor.WriteWarning(
                        "Found illegally referenced Assembly '{0}'. " +
                        "CorePlugins should never reference or use DualityEditor or any of its EditorPlugins. Consider moving the critical code to an EditorPlugin.",
                        asmName);
                }
            }

            // Try to retrieve all Types from the current Assembly
            Type[] exportedTypes;
            try
            {
                exportedTypes = plugin.PluginAssembly.GetExportedTypes();
            }
            catch (Exception e)
            {
                Logs.Editor.WriteError(
                    "Unable to analyze exported types because an error occured: {0}",
                    LogFormat.Exception(e));
                exportedTypes = null;
            }

            // Analyze exported types
            if (exportedTypes != null)
            {
                // Query Component types
                var cmpTypeQuery = from Type t in exportedTypes
                                   where typeof(Component).IsAssignableFrom(t)
                                   select t;
                foreach (var cmpType in cmpTypeQuery)
                {
                    // Scan for public Fields
                    FieldInfo[] fields = cmpType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                    if (fields.Length > 0)
                    {
                        Logs.Editor.WriteWarning(
                            "Found public fields in Component class '{0}': {1}. " +
                            "The usage of public fields is strongly discouraged in Component classes. Consider using properties instead.",
                            cmpType.GetTypeCSCodeName(true),
                            fields.ToString(f => LogFormat.FieldInfo(f, false), ", "));
                    }
                }
            }

            Logs.Editor.PopIndent();
        }
    }
}
