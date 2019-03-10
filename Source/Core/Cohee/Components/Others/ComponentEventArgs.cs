using System;

namespace Cohee
{
    /// <summary>
	/// Provides event arguments for <see cref="Cohee.Component"/>-related events.
	/// </summary>
	public class ComponentEventArgs : EventArgs
    {
        private Component cmp;
        /// <summary>
        /// [GET] The affected Component.
        /// </summary>
        public Component Component
        {
            get { return this.cmp; }
        }
        public ComponentEventArgs(Component cmp)
        {
            this.cmp = cmp;
        }
    }
}
