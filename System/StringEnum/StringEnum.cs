using System.Collections.Immutable;
using System.Reflection;

namespace Odin;

/// <summary>
/// Provides enum-like behaviour for a set of string values.
/// Usage: Inherit from StringEnum and add ONLY public const string members.
/// Use the Values static property to work with a list of all the string values.
/// Use HasValue(string value) to find out whether a particular string value is a member of Values.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public abstract class StringEnum<TEnum> where TEnum: StringEnum<TEnum> 
{
    /// <summary>
    /// Static field in generic type is fine since we'll get a different one for each TEnum when StringEnum is reified.
    /// </summary>
    private static ImmutableHashSet<string>? _values;
    
    // ReSharper disable once StaticMemberInGenericType
    private static readonly object ValuesReflectionLock = new();

    /// <summary>
    /// Returns the set of string values
    /// </summary>
    public static ImmutableHashSet<string> Values
    {
        get
        {
            if (_values is not null)
            {
                return _values;
            }

            lock (ValuesReflectionLock)
            {
                if (_values != null) return _values;
                List<string> nonDistinctValues = typeof(TEnum)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                    .Select(f => (string)f.GetValue(null)!)
                    .ToList();
                int numMembers = nonDistinctValues.Count;
                _values = nonDistinctValues.ToImmutableHashSet(StringComparer.Ordinal);
                if (numMembers != _values.Count)
                {
                    _values = null;
                    throw new NotSupportedException($"Duplicate string values found in {typeof(TEnum)}");
                }
                return _values;
            }
        }
    }

    /// <summary>
    /// Returns true if the value exists as one of the members of Values.
    /// Comparison is case-sensitive.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result HasValue(string? value)
    {
        StringComparer comparer = StringComparer.FromComparison(StringComparison.Ordinal);
        if (!Values.Contains(value, comparer))
        {
            return Result.Fail(NotAMemberMessage(value));
        }
        return Result.Succeed();
    }

    private static string NotAMemberMessage(string? value) =>
        $"\"{value}\" is not a valid member of StringEnum {typeof(TEnum)}. Valid members: {string.Join(", ", Values)}";
    
}
