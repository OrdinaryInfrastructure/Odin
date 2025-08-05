namespace Tests.Odin;

public static class TypeBuilder
{
    public static IReadOnlyList<Type> CreateTestTypes()
    {
        return new List<Type>()
        {
            typeof(string),
            typeof(int),
            typeof(object)
        };
    }
}