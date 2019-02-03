using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    /// <summary>
    /// Provides general information about this Cohee application / game.
    /// </summary>
    public class CoheeAppData
    {

        private string[] skipBackends = null;

        /// <summary>
        /// [GET / SET] An optional list of backend <see cref="Cohee.Backend.ICoheeBackend.Id"/> values to skip when loading.
        /// </summary>
        public string[] SkipBackends
        {
            get { return this.skipBackends; }
            set { this.skipBackends = value; }
        }

    }
}
