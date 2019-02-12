using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee.Cloning
{
    public enum CloneBehavior
    {
        /// <summary>
        /// The object will be handled automatically according to its Type properties and Attributes.
        /// </summary>
        Default,
        /// <summary>
        /// The object will be assigned by-reference, because external ownership is assumed.
        /// </summary>
        Reference,
        /// <summary>
        /// The object will be cloned deeply, because local ownership is assumed.
        /// </summary>
        ChildObject
    }
}
