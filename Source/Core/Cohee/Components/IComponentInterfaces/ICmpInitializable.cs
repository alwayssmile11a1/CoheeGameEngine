namespace Cohee
{
    /// <summary>
    /// Describes an interface for a <see cref="Component"/> that will receive events
    /// when it enters an active or inactive state.
    /// </summary>
    public interface ICmpInitializable
    {
        /// <summary>
        /// Called when the <see cref="Component"/> is now considered active when it wasn't before.
        /// 
        /// This can be the result of activating it, activating its GameObject, adding itsself or 
        /// its GameObject to the <see cref="Resources.Scene.Current"/> Scene, or entering a Scene in which 
        /// this Component is present.
        /// 
        /// Note that these events are fired both in game and editor context. To check which context
        /// is currently active, use <see cref="CoheeApp.ExecContext"/>.
        /// </summary>
        void OnActivate();
        /// <summary>
        /// Called when the <see cref="Component"/> is now considered inactive when it was active before.
        /// 
        /// This can be the result of deactivating it, deactivating its GameObject, removing itsself or 
        /// its GameObject from the <see cref="Resources.Scene.Current"/> Scene, or leaving a Scene in 
        /// which this Component is present.
        /// 
        /// Note that these events are fired both in game and editor context. To check which context
        /// is currently active, use <see cref="CoheeApp.ExecContext"/>.
        /// </summary>
        void OnDeactivate();
    }
}
