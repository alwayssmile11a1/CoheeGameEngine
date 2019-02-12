using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    /// <summary>
	/// Represents an object that can be de/activated and explicitly released / disposed
	/// </summary>
	public interface IManageableObject
    {
        /// <summary>
        /// [GET] Returns whether the object is considered disposed.
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// [GET] Returns whether the object is currently active.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        void Dispose();
    }
}
