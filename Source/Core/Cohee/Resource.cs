using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    public abstract class Resource
    {
        /// <summary>
        /// A Resource files extension.
        /// </summary>
        internal static readonly string FileExt = ".res";
        /// <summary>
        /// (Virtual) base path for Cohee's embedded default content.
        /// </summary>
        public static readonly string DefaultContentBasePath = "Default:";
    }
}
