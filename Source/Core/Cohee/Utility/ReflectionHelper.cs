using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    /// <summary>
	/// Provides reflection-related helper methods.
	/// </summary>
    public static class ReflectionHelper
    {
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
    }
}
