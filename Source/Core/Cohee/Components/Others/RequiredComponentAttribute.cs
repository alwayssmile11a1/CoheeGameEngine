﻿using System;
using System.Reflection;

namespace Cohee
{
    /// <summary>
    /// This attribute indicates a <see cref="Component">Components</see> requirement for another Component
    /// of a specific Type, that is attached to the same <see cref="GameObject"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredComponentAttribute : Attribute
    {
        private Type requiredType;
        private Type createDefaultType;

        /// <summary>
        /// The component type that is required by this component.
        /// </summary>
        public Type RequiredComponentType
        {
            get { return this.requiredType; }
        }
        /// <summary>
        /// The type that will be instantiated when automatically creating dependency components
        /// for this component. Defaults to <see cref="RequiredComponentType"/>.
        /// </summary>
        public Type CreateDefaultType
        {
            get { return this.createDefaultType; }
        }

        public RequiredComponentAttribute(Type requiredType) : this(requiredType, requiredType) { }
        public RequiredComponentAttribute(Type requiredType, Type createDefaultType)
        {
            requiredType = requiredType ?? typeof(Component);
            createDefaultType = createDefaultType ?? requiredType;

            // If the creation type doesn't satisfy the requirement, fallback to default.
            // An exception would be better, but throwing them in an attribute constructor
            // is a bit dangerous.
            if (!requiredType.GetTypeInfo().IsAssignableFrom(createDefaultType.GetTypeInfo()))
                createDefaultType = requiredType;

            this.requiredType = requiredType;
            this.createDefaultType = createDefaultType;
        }
    }
}
