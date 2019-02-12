using System;

namespace Cohee.Editor
{
    /// <summary>
	/// Provides general information about a members preferred editor behaviour.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false)]
    public class EditorHintFlagsAttribute : EditorHintAttribute
    {
        private MemberFlags flags;
        /// <summary>
        /// [GET] Flags that indicate the members general behaviour
        /// </summary>
        public MemberFlags Flags
        {
            get { return this.flags; }
        }
        public EditorHintFlagsAttribute(MemberFlags flags)
        {
            this.flags = flags;
        }
    }
}
