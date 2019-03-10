namespace Cohee
{
    /// <summary>
    /// Describes an interface for a <see cref="Component"/> that listens for
    /// its own <see cref="GameObject"/> attach and detach events. 
    /// 
    /// Attachment state persists serialization and initialization, so events
    /// will only be invoked when invoking <see cref="GameObject.AddComponent"/>
    /// or <see cref="GameObject.RemoveComponent"/> methods.
    /// </summary>
    public interface ICmpAttachmentListener
    {
        /// <summary>
        /// Called when the <see cref="Component"/> is added to a <see cref="GameObject"/>.
        /// </summary>
        void OnAddToGameObject();
        /// <summary>
        /// Called when the <see cref="Component"/> is removed from a <see cref="GameObject"/>.
        /// </summary>
        void OnRemoveFromGameObject();
    }
}
