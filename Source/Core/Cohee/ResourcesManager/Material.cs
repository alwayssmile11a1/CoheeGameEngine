using Cohee.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee.Resources
{
    /// <summary>
	/// Materials are standardized <see cref="BatchInfo">BatchInfos</see>, stored as a Resource. 
	/// Just like BatchInfo objects, they describe how an object, represented by a set of vertices, 
	/// looks like. Using Materials is generally more performant than using BatchInfos but not always
	/// reasonable, for example when there is a single, unique GameObject with a special appearance:
	/// This is a typical <see cref="BatchInfo"/> case.
	/// </summary>
	/// <seealso cref="BatchInfo"/>
    public class Material : Resource
    {

        private BatchInfo info = new BatchInfo();

        /// <summary>
        /// [GET] Returns the Materials internal <see cref="BatchInfo"/> instance, which can be used for rendering.
        /// </summary>
        public BatchInfo Info
        {
            get { return this.info; }
        }
    }
}
