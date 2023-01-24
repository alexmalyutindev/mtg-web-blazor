using System.Collections;

namespace MtgWeb.Core;

public static class ComponentsBucket
{
    public static void Add<T>(T component) where T : Component
    {
        Console.WriteLine(typeof(T));
        ComponentsBucket<T>.Add(component);
    }

    public static void Remove<T>(T component) where T : Component => 
        ComponentsBucket<T>.Remove(component);
}

public static class ComponentsBucket<T> where T : Component
{
    public static IEnumerable<T> Bucket => _bucket;
    private static T[] _bucket;

    static ComponentsBucket()
    {
        _bucket = Array.Empty<T>();
    }

    public static void Add(T component)
    {
        Array.Resize(ref _bucket, _bucket.Length + 1);
        _bucket[^1] = component;
    }

    public static void Remove(T component)
    {
        var index = Array.IndexOf(_bucket, component);
        if (index > 0)
        {
            _bucket[index] = _bucket[^1];
            Array.Resize(ref _bucket, _bucket.Length - 1);
        }
    }
}