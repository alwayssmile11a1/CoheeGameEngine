using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    public static class ExtMethodsPropertyInfo
    {
        public static bool IsPublic(this PropertyInfo property)
        {
            return
                (property.CanRead && property.GetMethod.IsPublic) ||
                (property.CanWrite && property.SetMethod.IsPublic);
        }
        public static bool IsStatic(this PropertyInfo property)
        {
            return
                (property.CanRead && property.GetMethod.IsStatic) ||
                (property.CanWrite && property.SetMethod.IsStatic);
        }
    }
}
