using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace DevTrends.ConfigurationExtensions;

internal class ReflectedGenericList
{
    private object List { get; init; }
    private MethodInfo DoAdd { get; init; }
    private Type GenericEnumerableType { get; init; }

    public ReflectedGenericList(Type containedType)
    {
        // We know List<T> definitely has a List<T>() constructor and an Add(T) method
        Type genericListType = typeof(List<>).MakeGenericType(containedType);
        var constructor = genericListType.GetConstructor(new Type[] { })!;
        List = constructor.Invoke(new object?[] { });
        DoAdd = genericListType.GetMethod("Add", new Type[] { containedType })!;
        GenericEnumerableType = typeof(IEnumerable<>).MakeGenericType(containedType);
    }

    public void Add(object? item)
    {
        DoAdd.Invoke(List, new object?[] { item });
    }

    public object AsContainerOfT(Type containerType)
    {
        // There may be other exceptions here...
        var creator = containerType.GetMethod("CreateRange", BindingFlags.Public | BindingFlags.Static, new Type[] { GenericEnumerableType });
        if (creator is null)
            return AsMutableContainerOfT(containerType);
        return creator.Invoke(null, new object[] { (IEnumerable)List })!;
    }

    private object AsMutableContainerOfT(Type containerType)
    {
        var constructor = containerType.GetConstructor(new Type[] { GenericEnumerableType });
        if (constructor is null)
            throw new ConfigurationBindException($"Unhandled type '{containerType.FullName}'");
        return constructor.Invoke(new object[] { (IEnumerable)List });
    }
}
