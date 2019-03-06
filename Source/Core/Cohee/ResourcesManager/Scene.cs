using Cohee.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee.Resources
{
    public sealed class Scene : Resource
    {
        /// <summary>
        /// [GET] Returns whether this <see cref="Scene"/> is currently <see cref="Activate">active</see>,
        /// i.e. in a state where it can update game simulation and be rendered.
        /// </summary>
        [EditorHintFlags(MemberFlags.Invisible)]
        public bool IsActive
        {
            get { return true; }
        }
    }
}
