using System.Reflection;

namespace Vogel.BuildingBlocks.Application.Extensions
{
    public static class ReflectionHelperExtensions
    {
        public static List<Type> GetClassesThatImplementing<T>(this Assembly assembly)
        {
            return GetClassesThatImplementing(assembly, typeof(T));
        }
        public static List<Type> GetClassesThatImplementing(this Assembly assembly,Type implementedInterface)
        {
           return assembly.GetTypes()
                .Where(x => x.IsClass)
                .Where(x => !x.IsAbstract)
                .Where(c => implementedInterface.IsAssignableFrom(c))
                .ToList();
        }

    }
}
