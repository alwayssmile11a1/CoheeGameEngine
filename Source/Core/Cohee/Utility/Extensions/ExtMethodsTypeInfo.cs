using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    public static class ExtMethodsTypeInfo
    {
        /// <summary>
        /// Returns a TypeInfos BaseType as a TypeInfo, or null if it was null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeInfo GetBaseTypeInfo(this TypeInfo type)
        {
            return type.BaseType != null ? type.BaseType.GetTypeInfo() : null;
        }

        /// <summary>
		/// Returns all fields that are declared within this Type, or any of its base Types.
		/// Includes public, non-public, static and instance fields.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		public static IEnumerable<FieldInfo> DeclaredFieldsDeep(this TypeInfo type)
		{
			IEnumerable<FieldInfo> result = Enumerable.Empty<FieldInfo>();

			while (type != null)
			{
				result = result.Concat(type.DeclaredFields);
				type = type.GetBaseTypeInfo();
			}

			return result;
		}

        /// <summary>
		/// Returns all members that are declared within this Type, or any of its base Types.
		/// Includes public, non-public, static and instance fields.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		public static IEnumerable<MemberInfo> DeclaredMembersDeep(this TypeInfo type)
        {
            IEnumerable<MemberInfo> result = Enumerable.Empty<MemberInfo>();

            while (type != null)
            {
                result = result.Concat(type.DeclaredMembers);
                type = type.GetBaseTypeInfo();
            }

            return result;
        }
    }
}
