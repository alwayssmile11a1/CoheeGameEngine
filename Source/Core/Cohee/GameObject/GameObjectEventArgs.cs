using System;

namespace Cohee
{
    /// <summary>
    /// Provides event arguments for <see cref="Cohee.GameObject"/>-related events.
    /// </summary>
    public class GameObjectEventArgs : EventArgs
    {
        private GameObject obj;
        /// <summary>
        /// [GET] The affected GameObject.
        /// </summary>
        public GameObject Object
        {
            get { return this.obj; }
        }
        public GameObjectEventArgs(GameObject obj)
        {
            this.obj = obj;
        }
    }
}
