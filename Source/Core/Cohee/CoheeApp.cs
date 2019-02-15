using Cohee.Backend;
using Cohee.IO;
using Cohee.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    public static class CoheeApp
    {
        /// <summary>
		/// Describes the context in which the current CoheeApp runs.
		/// </summary>
		public enum ExecutionContext
        {
            /// <summary>
            /// Cohee has been terminated. There is no guarantee that any object is still valid or usable.
            /// </summary>
            Terminated,
            /// <summary>
            /// The context in which Cohee is executed is unknown.
            /// </summary>
            Unknown,
            /// <summary>
            /// Cohee runs in a game environment.
            /// </summary>
            Game,
            /// <summary>
            /// Cohee runs in an editing environment.
            /// </summary>
            Editor
        }
        /// <summary>
        /// Describes the environment in which the current CoheeApp runs.
        /// </summary>
        public enum ExecutionEnvironment
        {
            /// <summary>
            /// The environment in which Cohee is executed is unknown.
            /// </summary>
            Unknown,
            /// <summary>
            /// Cohee runs in the CoheeLauncher
            /// </summary>
            Launcher,
            /// <summary>
            /// Cohee runs in the CoheeEditor
            /// </summary>
            Editor
        }

        public const string DataDirectory = "Data";
        public const string PluginDirectory = "Plugins";
        public const string CmdArgDebug = "debug";
        public const string CmdArgEditor = "editor";
        public const string CmdArgProfiling = "profile";
        public const string AppDataPath = "AppData.dat";
        public const string UserDataPath = "UserData.dat";
        

        private static bool initialized = false;
        private static bool runFromEditor = false;
        private static IAssemblyLoader assemblyLoader = null;
        private static ExecutionEnvironment environment = ExecutionEnvironment.Unknown;
        private static ExecutionContext execContext = ExecutionContext.Terminated;
        private static CorePluginManager corePluginManager = new CorePluginManager();
        private static ISystemBackend systemBackend = null;
        private static IGraphicsBackend graphicsBackend = null;
        private static IAudioBackend audioBackend = null;
        private static CoheeAppData appData = null;
        private static CoheeUserData userData = null;
        private static bool isUpdating = false;
        private static bool terminateScheduled = false;

        /// <summary>
        /// Called when the games UserData changes
        /// </summary>
        public static event EventHandler UserDataChanged = null;
        /// <summary>
        /// Called when the games AppData changes
        /// </summary>
        public static event EventHandler AppDataChanged = null;
        /// <summary>
        /// Called when Cohee is being terminated by choice (e.g. not because of crashes or similar).
        /// It is also called in an editor environment.
        /// </summary>
        public static event EventHandler Terminating = null;

        /// <summary>
        /// [GET] The core plugin manager that is used by Cohee. Don't use this unless you know exactly what you're doing.
        /// If you want to load a plugin, use the <see cref="CorePluginManager"/> from this property.
        /// If you want to load a non-plugin Assembly, use the <see cref="AssemblyLoader"/>.
        /// </summary>
        public static CorePluginManager CorePluginManager
        {
            get { return corePluginManager; }
        }
        /// <summary>
        /// [GET] The system backend that is used by Cohee. Don't use this unless you know exactly what you're doing.
        /// </summary>
        public static ISystemBackend SystemBackend
        {
            get { return systemBackend; }
        }
        /// <summary>
        /// [GET] The graphics backend that is used by Duality. Don't use this unless you know exactly what you're doing.
        /// </summary>
        public static IGraphicsBackend GraphicsBackend
        {
            get { return graphicsBackend; }
        }
        /// <summary>
        /// [GET] The audio backend that is used by Duality. Don't use this unless you know exactly what you're doing.
        /// </summary>
        public static IAudioBackend AudioBackend
        {
            get { return audioBackend; }
        }
        /// <summary>
        /// [GET] The plugin loader that is used by Cohee. Don't use this unless you know exactly what you're doing.
        /// If you want to load a plugin, use the <see cref="CorePluginManager"/>. 
        /// If you want to load a non-plugin Assembly, use the <see cref="IAssemblyLoader"/> from this property.
        /// </summary>
        public static IAssemblyLoader AssemblyLoader
        {
            get { return assemblyLoader; }
        }
        /// <summary>
		/// [GET] Returns the <see cref="ExecutionContext"/> in which this CoheeApp is currently running.
		/// </summary>
		public static ExecutionContext ExecContext
        {
            get { return execContext; }
            internal set
            {
                if (execContext != value)
                {
                    ExecutionContext previous = execContext;
                    execContext = value;

                    if (previous == ExecutionContext.Game && value != ExecutionContext.Game)
                        corePluginManager.InvokeGameEnded();

                    corePluginManager.InvokeExecContextChanged(previous);

                    if (previous != ExecutionContext.Game && value == ExecutionContext.Game)
                        corePluginManager.InvokeGameStarting();
                }
            }
        }
        /// <summary>
        /// [GET] Returns the <see cref="ExecutionEnvironment"/> in which this CoheeApp is currently running.
        /// </summary>
        public static ExecutionEnvironment ExecEnvironment
        {
            get { return environment; }
        }
        /// <summary>
		/// [GET / SET] Provides access to Cohee's current <see cref="CoheeAppData">application data</see>. This is never null.
		/// Any kind of data change event is fired as soon as you re-assign this property. Be sure to do that after changing its data.
		/// </summary>
		public static CoheeAppData AppData
        {
            get { return appData; }
            set
            {
                appData = value ?? new CoheeAppData();
                // We're currently missing direct changes without invoking this setter
                OnAppDataChanged();
            }
        }
        /// <summary>
        /// [GET / SET] Provides access to Cohee's current <see cref="CoheeUserData">user data</see>. This is never null.
        /// Any kind of data change event is fired as soon as you re-assign this property. Be sure to do that after changing its data.
        /// </summary>
        public static CoheeUserData UserData
        {
            get { return userData; }
            set
            {
                userData = value ?? new CoheeUserData();
                // We're currently missing direct changes without invoking this setter
                OnUserDataChanged();
            }
        }

        /// <summary>
		/// Initializes this CoheeApp. Should be called before performing any operations within Cohee.
		/// </summary>
		/// <param name="context">The <see cref="ExecutionContext"/> in which Duality runs.</param>
		/// <param name="commandLineArgs">
		/// Command line arguments to run this CoheeApp with. 
		/// Usually these are just the ones from the host application, passed on.
		/// </param>
		public static void Init(ExecutionEnvironment env, ExecutionContext context, IAssemblyLoader assemLoader, string[] commandLineArgs)
        {
            if (initialized) return;

            // Process command line options
            if (commandLineArgs != null)
            {
                // Enter debug mode
                if (commandLineArgs.Contains(CmdArgDebug)) System.Diagnostics.Debugger.Launch();
                // Run from editor
                if (commandLineArgs.Contains(CmdArgEditor)) runFromEditor = true;
            }

            // If the core was compiled in debug mode and a debugger is attached, log 
            // to the Debug channel, so we can put the VS output window to good use.
            #if DEBUG
            bool isDebugging = System.Diagnostics.Debugger.IsAttached;
            if (isDebugging)
            {
                // Only add a new Debug output if we don't already have one, and don't
                // log to a Console channel either. VS will automatically redirect Console
                // output to the Output window when debugging a non-Console application,
                // and we don't want to end up with double log entries.
                bool hasDebugOut = Logs.GlobalOutput.OfType<DebugLogOutput>().Any();
                bool hasConsoleOut = Logs.GlobalOutput.OfType<TextWriterLogOutput>().Any(w => w.GetType().Name.Contains("Console"));
                if (!hasDebugOut && !hasConsoleOut)
                {
                    Logs.AddGlobalOutput(new DebugLogOutput());
                }
            }
            #endif

            environment = env;
            execContext = context;

            // Initialize the plugin manager
            {
                assemblyLoader = assemLoader ?? new Cohee.Backend.Dummy.DummyAssemblyLoader();
                Logs.Core.Write("Using '{0}' to load plugins.", assemblyLoader.GetType().Name);

                assemblyLoader.Init();

                // Log assembly loading data for diagnostic purposes
                {
                    Logs.Core.Write("Currently Loaded Assemblies:" + Environment.NewLine + "{0}",
                        assemblyLoader.LoadedAssemblies.ToString(
                            assembly => "  " + LogFormat.Assembly(assembly),
                            Environment.NewLine));
                    Logs.Core.Write("Plugin Base Directories:" + Environment.NewLine + "{0}",
                        assemblyLoader.BaseDirectories.ToString(
                            path => "  " + path,
                            Environment.NewLine));
                    Logs.Core.Write("Available Assembly Paths:" + Environment.NewLine + "{0}",
                        assemblyLoader.AvailableAssemblyPaths.ToString(
                            path => "  " + path,
                            Environment.NewLine));
                }

                corePluginManager.Init(assemblyLoader);
                corePluginManager.PluginsRemoving += pluginManager_PluginsRemoving;
                corePluginManager.PluginsRemoved += pluginManager_PluginsRemoved;
            }

            // Load all plugins. This needs to be done first, so backends and Types can be located.
            corePluginManager.LoadPlugins();

            // Initialize the system backend for system info and file system access
            InitBackend(out systemBackend);

            // Load application and user data and submit a change event, so all settings are applied
            LoadAppData();
            LoadUserData();
            OnAppDataChanged();
            OnUserDataChanged();

            // Initialize the graphics backend
            InitBackend(out graphicsBackend);

            // Initialize the audio backend
            InitBackend(out audioBackend);
            //sound = new SoundDevice();

            // Initialize all core plugins, this may allocate Resources or establish references between plugins
            corePluginManager.InitPlugins();

            initialized = true;

            // Write environment specs as a debug log
            Logs.Core.Write(
                "DualityApp initialized" + Environment.NewLine +
                "Debug Mode: {0}" + Environment.NewLine +
                "Command line arguments: {1}",
                System.Diagnostics.Debugger.IsAttached,
                commandLineArgs != null ? commandLineArgs.ToString(", ") : "null");
        }

        public static void Terminate()
        {
            if (!initialized) return;
            if (isUpdating)
            {
                terminateScheduled = true;
                return;
            }

            if (environment == ExecutionEnvironment.Editor && execContext == ExecutionContext.Game)
            {
                //Scene.Current.Dispose();
                Logs.Core.Write("CoheeApp terminated in sandbox mode.");
                terminateScheduled = false;
                return;
            }

            if (execContext != ExecutionContext.Editor)
            {
                OnTerminating();
                SaveUserData();
            }

            // Signal that the game simulation has ended.
            if (execContext == ExecutionContext.Game)
                corePluginManager.InvokeGameEnded();

            // Dispose all content that is still loaded
            ContentProvider.ClearContent(true);

            // Discard plugin data (Resources, current Scene) ahead of time. Otherwise, it'll get shut down in ClearPlugins, after the backend is gone.
            corePluginManager.DiscardPluginData();

            //sound.Dispose();
            //sound = null;
            ShutdownBackend(ref graphicsBackend);
            ShutdownBackend(ref audioBackend);

            corePluginManager.ClearPlugins();

            //// Since this performs file system operations, it needs to happen before shutting down the system backend.
            //Profile.SaveTextReport(environment == ExecutionEnvironment.Editor ? "perflog_editor.txt" : "perflog.txt");

            ShutdownBackend(ref systemBackend);

            // Shut down the plugin manager and plugin loader
            corePluginManager.Terminate();
            corePluginManager.PluginsRemoving -= pluginManager_PluginsRemoving;
            corePluginManager.PluginsRemoved -= pluginManager_PluginsRemoved;
            assemblyLoader.Terminate();
            assemblyLoader = null;

            Logs.Core.Write("CoheeApp terminated");

            initialized = false;
            execContext = ExecutionContext.Terminated;
        }

  //      /// <summary>
		///// Initializes the part of Cohee that requires a valid rendering context. 
		///// Should be called before performing any rendering related operations with Cohee.
		///// Is called implicitly when using <see cref="OpenWindow"/>.
		///// </summary>
  //      public static void InitPostWindow()
  //      {
  //          DefaultContent.Init();

  //          // Post-Window init is the last thing that happens before loading game
  //          // content and entering simulation. When done in a game context, notify
  //          // plugins that the game is about to start - otherwise, exec context changes
  //          // will trigger the same code later.
  //          if (execContext == ExecutionContext.Game)
  //              corePluginManager.InvokeGameStarting();
  //      }

        //      /// <summary>
        //      /// Opens up a window for Cohee to render into. This also initializes the part of Duality that requires a 
        //      /// valid rendering context. Should be called before performing any rendering related operations with Duality.
        //      /// </summary>
        //      public static INativeWindow OpenWindow(WindowOptions options)
        //      {
        //          if (!initialized) throw new InvalidOperationException("Can't initialize graphics / rendering because Cohee itself isn't initialized yet.");

        //          Logs.Core.Write("Opening Window...");
        //          Logs.Core.PushIndent();
        //          INativeWindow window = graphicsBackend.CreateWindow(options);
        //          Logs.Core.PopIndent();

        //          InitPostWindow();

        //          return window;
        //      }

        /// <summary>
        /// Triggers Cohee to (re)load its <see cref="CoheeAppData"/>.
        /// </summary>
        public static void LoadAppData()
        {
            appData = Serializer.TryReadObject<CoheeAppData>(AppDataPath) ?? new CoheeAppData();
        }
        /// <summary>
        /// Triggers Cohee to (re)load its <see cref="CoheeUserData"/>.
        /// </summary>
        public static void LoadUserData()
        {
            string path = UserDataPath;
            if (!FileOp.Exists(path) || execContext == ExecutionContext.Editor || runFromEditor) path = "DefaultUserData.dat";
            userData = Serializer.TryReadObject<CoheeUserData>(path) ?? new CoheeUserData();
        }
        /// <summary>
        /// Triggers Cohee to save its <see cref="CoheeAppData"/>.
        /// </summary>
        public static void SaveAppData()
        {
            Serializer.WriteObject(appData, AppDataPath, typeof(XmlSerializer));
        }
        /// <summary>
        /// Triggers Cohee to save its <see cref="CoheeUserData"/>.
        /// </summary>
        public static void SaveUserData()
        {
            Serializer.WriteObject(userData, UserDataPath, typeof(XmlSerializer));
            if (execContext == ExecutionContext.Editor)
            {
                Serializer.WriteObject(userData, "DefaultUserData.dat", typeof(XmlSerializer));
            }
        }
        private static void OnAppDataChanged()
        {
            if (AppDataChanged != null)
                AppDataChanged(null, EventArgs.Empty);
        }

        private static void OnUserDataChanged()
        {
            if (UserDataChanged != null)
                UserDataChanged(null, EventArgs.Empty);
        }

        private static void OnTerminating()
        {
            if (Terminating != null)
                Terminating(null, EventArgs.Empty);
        }

        internal static void InitBackend<T>(out T target, Func<Type, IEnumerable<TypeInfo>> typeFinder = null) where T : class, ICoheeBackend
        {
            if (typeFinder == null) typeFinder = GetAvailCoheeTypes;

            Logs.Core.Write("Initializing {0}...", LogFormat.Type(typeof(T)));
            Logs.Core.PushIndent();

            // Generate a list of available backends for evaluation
            List<ICoheeBackend> backends = new List<ICoheeBackend>();
            foreach (TypeInfo backendType in typeFinder(typeof(ICoheeBackend)))
            {
                if (backendType.IsInterface) continue;
                if (backendType.IsAbstract) continue;
                if (!backendType.IsClass) continue;
                if (!typeof(T).GetTypeInfo().IsAssignableFrom(backendType)) continue;

                ICoheeBackend backend = backendType.CreateInstanceOf() as ICoheeBackend;
                if (backend == null)
                {
                    Logs.Core.WriteWarning("Unable to create an instance of {0}. Skipping it.", backendType.FullName);
                    continue;
                }
                backends.Add(backend);
            }

            // Sort backends from best to worst
            backends.StableSort((a, b) => b.Priority > a.Priority ? 1 : -1);

            // Try to initialize each one and select the first that works
            T selectedBackend = null;
            foreach (T backend in backends)
            {
                if (appData != null &&
                    appData.SkipBackends != null &&
                    appData.SkipBackends.Any(s => string.Equals(s, backend.Id, StringComparison.OrdinalIgnoreCase)))
                {
                    Logs.Core.Write("Backend '{0}' skipped because of AppData settings.", backend.Name);
                    continue;
                }

                bool available = false;
                try
                {
                    available = backend.CheckAvailable();
                    if (!available)
                    {
                        Logs.Core.Write("Backend '{0}' reports to be unavailable. Skipping it.", backend.Name);
                    }
                }
                catch (Exception e)
                {
                    available = false;
                    Logs.Core.WriteWarning("Backend '{0}' failed the availability check with an exception: {1}", backend.Name, LogFormat.Exception(e));
                }
                if (!available) continue;

                Logs.Core.Write("{0}...", backend.Name);
                Logs.Core.PushIndent();
                {
                    try
                    {
                        backend.Init();
                        selectedBackend = backend;
                    }
                    catch (Exception e)
                    {
                        Logs.Core.WriteError("Failed: {0}", LogFormat.Exception(e));
                    }
                }
                Logs.Core.PopIndent();

                if (selectedBackend != null)
                    break;
            }

            // If we found a proper backend and initialized it, add it to the list of active backends
            if (selectedBackend != null)
            {
                target = selectedBackend;

                TypeInfo selectedBackendType = selectedBackend.GetType().GetTypeInfo();
                corePluginManager.LockPlugin(selectedBackendType.Assembly);
            }
            else
            {
                target = null;
            }

            Logs.Core.PopIndent();
        }

        internal static void ShutdownBackend<T>(ref T backend) where T : class, ICoheeBackend
        {
            if (backend == null) return;

            Logs.Core.Write("Shutting down {0}...", backend.Name);
            Logs.Core.PushIndent();
            {
                try
                {
                    backend.Shutdown();

                    TypeInfo backendType = backend.GetType().GetTypeInfo();
                    corePluginManager.UnlockPlugin(backendType.Assembly);

                    backend = null;
                }
                catch (Exception e)
                {
                    Logs.Core.WriteError("Failed: {0}", LogFormat.Exception(e));
                }
            }
            Logs.Core.PopIndent();
        }

        /// <summary>
        /// Enumerates all available Cohee <see cref="System.Type">Types</see> that are assignable
        /// to the specified Type. 
        /// </summary>
        /// <param name="baseType">The base type to use for matching the result types.</param>
        /// <returns>An enumeration of all Duality types deriving from the specified type.</returns>
        /// <example>
        /// The following code logs all available kinds of <see cref="Duality.Components.Renderer">Renderers</see>:
        /// <code>
        /// var rendererTypes = CoheeApp.GetAvailCoheeTypes(typeof(Cohee.Components.Renderer));
        /// foreach (Type rt in rendererTypes)
        /// {
        /// 	Logs.Core.Write("Renderer Type '{0}' from Assembly '{1}'", LogFormat.Type(rt), rt.Assembly.FullName);
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<TypeInfo> GetAvailCoheeTypes(Type baseType)
        {
            return corePluginManager.GetTypes(baseType);
        }

        /// <summary>
        /// Enumerates all currently loaded assemblies that are part of Cohee, i.e. Cohee itsself and all loaded plugins.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetCoheeAssemblies()
        {
            return corePluginManager.GetAssemblies();
        }

        //private static void pluginManager_PluginsRemoving(object sender, CoheePluginEventArgs e)
        //{
        //    // Save user and app data, they'll be reloaded after plugin reload is done,
        //    // as they can reference plugin data as well.
        //    SaveUserData();
        //    SaveAppData();

        //    // Dispose static Resources that could reference plugin data
        //    VisualLogs.ClearAll();
        //    if (!Scene.Current.IsEmpty)
        //        Scene.Current.Dispose();

        //    // Gather all other Resources that could reference plugin data
        //    List<Resource> pluginContent = new List<Resource>();
        //    Assembly coreAssembly = typeof(Resource).GetTypeInfo().Assembly;
        //    foreach (Resource resource in ContentProvider.GetLoadedContent<Resource>())
        //    {
        //        if (resource.IsDefaultContent) continue;

        //        Assembly assembly = resource.GetType().GetTypeInfo().Assembly;
        //        bool canReferencePluginData =
        //            resource is Prefab ||
        //            resource is Scene ||
        //            assembly != coreAssembly;

        //        if (canReferencePluginData)
        //            pluginContent.Add(resource);
        //    }

        //    // Dispose gathered content to avoid carrying over old instances by accident
        //    foreach (Resource r in pluginContent)
        //        ContentProvider.RemoveContent(r);
        //}
        //private static void pluginManager_PluginsRemoved(object sender, CoheePluginEventArgs e)
        //{
        //    // Clean globally cached type data
        //    ImageCodec.ClearTypeCache();
        //    ObjectCreator.ClearTypeCache();
        //    ReflectionHelper.ClearTypeCache();
        //    Component.RequireMap.ClearTypeCache();
        //    Component.ExecOrder.ClearTypeCache();
        //    Serializer.ClearTypeCache();
        //    CloneProvider.ClearTypeCache();

        //    // Clean input sources that a disposed Assembly forgot to unregister.
        //    foreach (CorePlugin plugin in e.Plugins)
        //        CleanInputSources(plugin.PluginAssembly);

        //    // Clean event bindings that are still linked to the disposed Assembly.
        //    foreach (CorePlugin plugin in e.Plugins)
        //        CleanEventBindings(plugin.PluginAssembly);

        //    // Reload user and app data
        //    LoadAppData();
        //    LoadUserData();
        //}
    }
}
