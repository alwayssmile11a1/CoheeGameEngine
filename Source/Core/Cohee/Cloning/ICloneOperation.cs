namespace Cohee.Cloning
{
    /// <summary>
    /// Cloning system interface that allows an <see cref="ICloneExplicit"/> or <see cref="ICloneSurrogate"/>
    /// to take part in the copy step of a cloning operation. The purpose of the copy step is to synchronously
    /// walk source and target object graphs while copying all data from source to target.
    /// </summary>
    public interface ICloneOperation
    {
        /// <summary>
        /// [GET] The context of this cloning operation, which can provide additional settings.
        /// </summary>
        CloneProviderContext Context { get; }

        /// <summary>
        /// Retrieves the target object that is mapped to the specified source object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        T GetTarget<T>(T source) where T : class;
        /// <summary>
        /// Returns true if the specified object is part of the target object graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        bool IsTarget<T>(T target) where T : class;

        /// <summary>
        /// Walks the object graph of the specified instance from the source graph, while copying its data to the graph that
        /// is spanned by its target object. May re-assign the target object in the process.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">An object from the source graph that will be copied by the cloning system.</param>
        /// <param name="target">The object's equivalent from the target graph to which data will be copied.</param>
        void HandleObject<T>(T source, ref T target) where T : class;
        /// <summary>
        /// Walks the object graph of the specified data structure from the source graph, while copying its data to the graph that
        /// is spanned by its target struct. May re-assign the target struct in the process.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A struct from the source graph that will be copied by the cloning system.</param>
        /// <param name="target">The struct's equivalent from the target graph to which data will be copied.</param>
        void HandleValue<T>(ref T source, ref T target) where T : struct;
    }
}
