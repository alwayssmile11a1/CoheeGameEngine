using System;
using Cohee.Drawing;

namespace Cohee.Backend
{
    /// <summary>
	/// Represents a GPU buffer for storing data such as vertices or indices.
	/// </summary>
	public interface INativeGraphicsBuffer : IDisposable
    {
        /// <summary>
        /// [GET] The kind of data that is stored in this buffer.
        /// </summary>
        GraphicsBufferType BufferType { get; }
        /// <summary>
        /// [GET] The length of the buffers available storage space, in bytes.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Sets up an empty storage with the specified size.
        /// </summary>
        /// <param name="size"></param>
        void SetupEmpty(int size);
        /// <summary>
        /// Uploads the specified data block into this buffer, replacing all previous contents.
        /// If not done before, this method will automatically allocate the required storage space.
        /// </summary>
        /// <param name="data">A pointer to the beginning of the data block to upload.</param>
        /// <param name="size">The size of the data block to upload, in bytes.</param>
        void LoadData(IntPtr data, int size);
        /// <summary>
        /// Uploads the specified data block into a subsection of this buffer, keeping all other content.
        /// </summary>
        /// <param name="offset">A memory offset from the beginning of the existing buffer.</param>
        /// <param name="data">A pointer to the beginning of the data block to upload.</param>
        /// <param name="size">The size of the data block to upload, in bytes.</param>
        void LoadSubData(IntPtr offset, IntPtr data, int size);
    }
}
