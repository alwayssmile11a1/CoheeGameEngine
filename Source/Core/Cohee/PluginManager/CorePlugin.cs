using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    public abstract class CorePlugin : CoheePlugin
    {
        /// <summary>
        /// Called when initializing the plugin. It is guaranteed that all plugins have been loaded at this point, so
        /// this is the ideal place to establish communication with other plugins or load Resources that may rely on them.
        /// It is NOT defined whether or not other plugins have been initialized yet.
        /// </summary>
        internal protected virtual void InitPlugin() { }
        /// <summary>
        /// Called before Cohee updates the game simulation.
        /// </summary>
        internal protected virtual void OnBeforeUpdate() { }
        /// <summary>
        /// Called after Cohee updates the game simulation.
        /// </summary>
        internal protected virtual void OnAfterUpdate() { }
        /// <summary>
        /// Called when Cohee's <see cref="CoheeApp.ExecutionContext"/> changes.
        /// </summary>
        internal protected virtual void OnExecContextChanged(CoheeApp.ExecutionContext previousContext) { }
        /// <summary>
        /// Called right before game simulation starts, e.g. when running the launcher or entering editor sandbox mode.
        /// </summary>
        internal protected virtual void OnGameStarting() { }
        /// <summary>
        /// Called right after game simulation has ended, e.g. by closing the launcher or exiting editor sandbox mode.
        /// </summary>
        internal protected virtual void OnGameEnded() { }
    }
}
