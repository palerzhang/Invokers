using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtils
{
    public class InitUtils
    {
        public static List<T> EmptyListIfNull<T>(List<T> list)
        {
            if (list == null)
            {
                return new List<T>();
            }
            else
            {
                return list;
            }
        }

        public static T[] EmptyArrayIfNull<T>(T[] list)
        {
            if (list == null)
            {
                return new T[] { };
            }
            else
            {
                return list;
            }
        }

        /// <summary>
        /// Get the subclasses of a base class in a namespace, including subnamespaces.
        /// </summary>
        /// <param name="baseClassType">Type of the base class.</param>
        /// <param name="inNamespace">Namespace to inspect in.</param>
        /// <param name="isIncludingAbstract">Whether to include abstract classes or not.</param>
        /// <returns></returns>
        public static List<Type> GetSubclassesInNameSpace(Type baseClassType, string inNamespace,
            bool isIncludingAbstract = false)
        {
            Type[] allClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass & IncludeSubnamespace(t, inNamespace)).ToArray();
            List<Type> classesList = new List<Type>();
            foreach (Type type in allClasses)
            {
                if (type.IsSubclassOf(baseClassType) && (!type.IsAbstract || isIncludingAbstract))
                {
                    classesList.Add(type);
                }
            }
            return classesList;
        }

        private static bool IncludeSubnamespace(Type t, string inNamespace)
        {
            if (t.Namespace != null && (t.Namespace == inNamespace || t.Namespace.StartsWith(inNamespace + ".")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
