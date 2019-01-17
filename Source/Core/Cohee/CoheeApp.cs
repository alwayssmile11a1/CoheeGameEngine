using Cohee.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
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


        private static CorePluginManager corePluginManager = new CorePluginManager();
        private static ISystemBackend systemBackend = null;


        /// <summary>
        /// [GET] The plugin manager that is used by Duality. Don't use this unless you know exactly what you're doing.
        /// If you want to load a plugin, use the <see cref="CorePluginManager"/> from this property.
        /// If you want to load a non-plugin Assembly, use the <see cref="AssemblyLoader"/>.
        /// </summary>
        public static CorePluginManager CPluginManager
        {
            get { return corePluginManager; }
        }

        /// <summary>
        /// [GET] The system backend that is used by Duality. Don't use this unless you know exactly what you're doing.
        /// </summary>
        public static ISystemBackend SystemBackend
        {
            get { return systemBackend; }
        }

        /// <summary>
		/// Initializes this DualityApp. Should be called before performing any operations within Duality.
		/// </summary>
		/// <param name="context">The <see cref="ExecutionContext"/> in which Duality runs.</param>
		/// <param name="commandLineArgs">
		/// Command line arguments to run this DualityApp with. 
		/// Usually these are just the ones from the host application, passed on.
		/// </param>
		public static void Init(ExecutionEnvironment env, ExecutionContext context, IAssemblyLoader plugins, string[] commandLineArgs)
        {

        }

        public static void Terminate()
        {
            //if (!initialized) return;
            //if (isUpdating)
            //{
            //    terminateScheduled = true;
            //    return;
            //}

            //if (environment == ExecutionEnvironment.Editor && execContext == ExecutionContext.Game)
            //{
            //    Scene.Current.Dispose();
            //    Logs.Core.Write("DualityApp terminated in sandbox mode.");
            //    terminateScheduled = false;
            //    return;
            //}

            //if (execContext != ExecutionContext.Editor)
            //{
            //    OnTerminating();
            //    SaveUserData();
            //}

            //// Signal that the game simulation has ended.
            //if (execContext == ExecutionContext.Game)
            //    pluginManager.InvokeGameEnded();

            //// Dispose all content that is still loaded
            //ContentProvider.ClearContent();

            //// Discard plugin data (Resources, current Scene) ahead of time. Otherwise, it'll get shut down in ClearPlugins, after the backend is gone.
            //pluginManager.DiscardPluginData();

            //sound.Dispose();
            //sound = null;
            //ShutdownBackend(ref graphicsBack);
            //ShutdownBackend(ref audioBack);
            //pluginManager.ClearPlugins();

            //// Since this performs file system operations, it needs to happen before shutting down the system backend.
            //Profile.SaveTextReport(environment == ExecutionEnvironment.Editor ? "perflog_editor.txt" : "perflog.txt");

            //ShutdownBackend(ref systemBack);

            //// Shut down the plugin manager and plugin loader
            //pluginManager.Terminate();
            //pluginManager.PluginsRemoving -= pluginManager_PluginsRemoving;
            //pluginManager.PluginsRemoved -= pluginManager_PluginsRemoved;
            //assemblyLoader.Terminate();
            //assemblyLoader = null;

            //Logs.Core.Write("DualityApp terminated");

            //initialized = false;
            //execContext = ExecutionContext.Terminated;
        }


        public static void InitPostWindow()
        {

        }
    }
}
