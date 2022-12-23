Console.WriteLine("HW");

static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}