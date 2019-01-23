using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cohee
{
    /// <summary>
	/// Provides reflection-related helper methods.
	/// </summary>
    public static class ReflectionHelper
    {
        private static Dictionary<MemberInfo, Attribute[]> customMemberAttribCache = new Dictionary<MemberInfo, Attribute[]>();

        /// <summary>
		/// Returns all custom attributes of the specified Type that are attached to the specified member.
		/// Inherites attributes are returned as well. This method is usually faster than comparable .Net methods,
		/// because it caches previous results internally.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="member"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetAttributesCached<T>(this MemberInfo member) where T : Attribute
        {
            lock (customMemberAttribCache)
            {
                Attribute[] result;
                if (!customMemberAttribCache.TryGetValue(member, out result))
                {
                    result = member.GetCustomAttributes(true).OfType<Attribute>().ToArray();

                    // If it's a Type, also check implemented interfaces for (EditorHint) attributes
                    if (member is TypeInfo)
                    {
                        TypeInfo typeInfo = member as TypeInfo;
                        IEnumerable<Attribute> query = result;
                        Type[] interfaces = typeInfo.ImplementedInterfaces.ToArray();
                        if (interfaces.Length > 0)
                        {
                            bool addedAny = false;
                            foreach (Type interfaceType in interfaces)
                            {
                                TypeInfo interfaceTypeInfo = interfaceType.GetTypeInfo();
                                IEnumerable<Attribute> subQuery = GetAttributesCached<Editor.EditorHintAttribute>(interfaceTypeInfo);
                                if (subQuery.Any())
                                {
                                    query = query.Concat(subQuery);
                                    addedAny = true;
                                }
                            }
                            if (addedAny)
                            {
                                result = query.Distinct().ToArray();
                            }
                        }
                    }
                    // If it's a member, check if it is an interface implementation and add their (EditorHint) attributes as well
                    else
                    {
                        TypeInfo declaringTypeInfo = member.DeclaringType == null ? null : member.DeclaringType.GetTypeInfo();
                        if (declaringTypeInfo != null && !declaringTypeInfo.IsInterface)
                        {
                            IEnumerable<Attribute> query = result;
                            Type[] interfaces = declaringTypeInfo.ImplementedInterfaces.ToArray();
                            if (interfaces.Length > 0)
                            {
                                bool addedAny = false;
                                foreach (Type interfaceType in interfaces)
                                {
                                    TypeInfo interfaceTypeInfo = interfaceType.GetTypeInfo();
                                    foreach (MemberInfo interfaceMemberInfo in interfaceTypeInfo.DeclaredMembersDeep())
                                    {
                                        if (interfaceMemberInfo.Name != member.Name) continue;
                                        IEnumerable<Attribute> subQuery = GetAttributesCached<Editor.EditorHintAttribute>(interfaceMemberInfo);
                                        if (subQuery.Any())
                                        {
                                            query = query.Concat(subQuery);
                                            addedAny = true;
                                        }
                                    }
                                }
                                if (addedAny)
                                {
                                    result = query.Distinct().ToArray();
                                }
                            }
                        }
                    }

                    // Mind the result for later. Don't do this twice.				
                    customMemberAttribCache[member] = result;
                }

                if (typeof(T) == typeof(Attribute))
                    return result as IEnumerable<T>;
                else
                    return result.OfType<T>();
            }
        }

        /// <summary>
        /// Returns all custom attributes of the specified Type that are attached to the specified member.
        /// Inherites attributes are returned as well. This method is usually faster than comparable .Net methods, 
        /// because it caches previous results internally.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool HasAttributeCached<T>(this MemberInfo member) where T : Attribute
        {
            return GetAttributesCached<T>(member).Any();
        }

        /// <summary>
		/// Returns the short version of an Assembly name.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static string GetShortAssemblyName(this Assembly assembly)
        {
            return assembly.FullName.Split(',')[0];
        }

        /// <summary>
        /// Returns the short version of an Assembly name.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static string GetShortAssemblyName(string assemblyName)
        {
            return assemblyName.Split(',')[0];
        }

        /// <summary>
		/// Returns a string describing a certain Type.
		/// </summary>
		/// <param name="type">The Type to describe</param>
		/// <returns></returns>
		public static string GetTypeCSCodeName(this Type type, bool shortName = false)
        {
            StringBuilder typeStr = new StringBuilder();

            if (type.IsGenericParameter)
            {
                return type.Name;
            }
            if (type.IsArray)
            {
                typeStr.Append(GetTypeCSCodeName(type.GetElementType(), shortName));
                typeStr.Append('[');
                typeStr.Append(',', type.GetArrayRank() - 1);
                typeStr.Append(']');
            }
            else
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                Type[] genArgs = typeInfo.IsGenericType ? typeInfo.GenericTypeArguments : null;

                if (type.IsNested)
                {
                    Type declType = type.DeclaringType;
                    TypeInfo declTypeInfo = declType.GetTypeInfo();

                    if (declTypeInfo.IsGenericTypeDefinition)
                    {
                        Array.Resize(ref genArgs, declTypeInfo.GenericTypeArguments.Length);
                        declType = declTypeInfo.MakeGenericType(genArgs);
                        declTypeInfo = declType.GetTypeInfo();
                        genArgs = type.GenericTypeArguments.Skip(genArgs.Length).ToArray();
                    }
                    string parentName = GetTypeCSCodeName(declType, shortName);

                    string[] nestedNameToken = shortName ? type.Name.Split('+') : type.FullName.Split('+');
                    string nestedName = nestedNameToken[nestedNameToken.Length - 1];

                    int genTypeSepIndex = nestedName.IndexOf("[[", StringComparison.Ordinal);
                    if (genTypeSepIndex != -1) nestedName = nestedName.Substring(0, genTypeSepIndex);
                    genTypeSepIndex = nestedName.IndexOf('`');
                    if (genTypeSepIndex != -1) nestedName = nestedName.Substring(0, genTypeSepIndex);

                    typeStr.Append(parentName);
                    typeStr.Append('.');
                    typeStr.Append(nestedName);
                }
                else
                {
                    if (shortName)
                        typeStr.Append(type.Name.Split(new[] { '`' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace('+', '.'));
                    else
                        typeStr.Append(type.FullName.Split(new[] { '`' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace('+', '.'));
                }

                if (genArgs != null && genArgs.Length > 0)
                {
                    if (typeInfo.IsGenericTypeDefinition)
                    {
                        typeStr.Append('<');
                        typeStr.Append(',', genArgs.Length - 1);
                        typeStr.Append('>');
                    }
                    else if (typeInfo.IsGenericType)
                    {
                        typeStr.Append('<');
                        for (int i = 0; i < genArgs.Length; i++)
                        {
                            typeStr.Append(GetTypeCSCodeName(genArgs[i], shortName));
                            if (i < genArgs.Length - 1)
                                typeStr.Append(',');
                        }
                        typeStr.Append('>');
                    }
                }
            }

            return typeStr.Replace('+', '.').ToString();
        }

        /// <summary>
        /// Returns the short version of an Assembly name.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static string GetShortAssemblyName(this AssemblyName assemblyName)
        {
            return assemblyName.FullName.Split(',')[0];
        }

        /// <summary>
        /// Returns a string describing a certain Type.
        /// </summary>
        /// <param name="T">The Type to describe</param>
        /// <returns></returns>
        public static string GetTypeId(this Type T)
        {
            return T.FullName != null ? Regex.Replace(T.FullName, @"(, [^\]\[]*)", "") : T.Name;
        }

    }
}
