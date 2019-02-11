namespace Cohee.Resources
{
    /// <summary>
    /// The type of a <see cref="Shader">shader</see> variable.
    /// </summary>
    public enum ShaderFieldType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// A <see cref="System.Boolean"/> variable.
        /// </summary>
        Bool,
        /// <summary>
        /// A <see cref="System.Int32"/> variable.
        /// </summary>
        Int,
        /// <summary>
        /// A <see cref="System.Single"/> variable.
        /// </summary>
        Float,

        /// <summary>
        /// A two-dimensional vector with <see cref="System.Single"/> precision.
        /// </summary>
        Vec2,
        /// <summary>
        /// A three-dimensional vector with <see cref="System.Single"/> precision.
        /// </summary>
        Vec3,
        /// <summary>
        /// A four-dimensional vector with <see cref="System.Single"/> precision.
        /// </summary>
        Vec4,

        /// <summary>
        /// A 2x2 matrix with <see cref="System.Single"/> precision.
        /// </summary>
        Mat2,
        /// <summary>
        /// A 3x3 matrix with <see cref="System.Single"/> precision.
        /// </summary>
        Mat3,
        /// <summary>
        /// A 4x4 matrix with <see cref="System.Single"/> precision.
        /// </summary>
        Mat4,

        /// <summary>
        /// Represents a texture binding and provides lookups.
        /// </summary>
        Sampler2D
    }
}
