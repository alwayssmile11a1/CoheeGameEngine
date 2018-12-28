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
    }
}
