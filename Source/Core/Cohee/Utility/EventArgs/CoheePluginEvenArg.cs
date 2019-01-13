using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{ 
    /// <summary>
	/// Provides event arguments related to <see cref="Cohee.CorePlugin"/> instances.
	/// </summary>
	public class CoheePluginEventArgs : EventArgs
    {
        private CoheePlugin[] plugins;
        public IReadOnlyList<CoheePlugin> Plugins
        {
            get { return this.plugins; }
        }
        public CoheePluginEventArgs(IEnumerable<CoheePlugin> plugins)
        {
            this.plugins =
                (plugins ?? Enumerable.Empty<CoheePlugin>())
                .NotNull()
                .Distinct()
                .ToArray();
        }
    }
}
