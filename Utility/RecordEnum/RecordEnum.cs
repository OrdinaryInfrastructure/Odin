using System.Collections.Immutable;
using System.Reflection;

namespace Odin.Utility;

/// <summary>
/// Provides enum-like behaviour for a set of record values.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public abstract class RecordEnum<TEnum, TMember> where TEnum: RecordEnum<TEnum,TMember>, IEquatable<RecordEnum<TEnum,TMember>>
{
    /// <summary>
    /// Static field in generic type is fine since we'll get a different one for each TEnum when StringEnum is reified.
    /// </summary>
    private static ImmutableHashSet<RecordEnum<TEnum, TMember>>? _values;
    
    // ReSharper disable once StaticMemberInGenericType
    private static readonly object ValuesReflectionLock = new ();

    /// <summary>
    /// Returns the set of string values
    /// </summary>
    public static ImmutableHashSet<RecordEnum<TEnum, TMember>> Values
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
                List<RecordEnum<TEnum,TMember>> nonDistinctValues = typeof(TEnum)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                    .Select(f => (RecordEnum<TEnum,TMember>) f.GetValue(null)!)
                    .ToList();
                int numMembers = nonDistinctValues.Count;
                _values = nonDistinctValues.ToImmutableHashSet();
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
    public static Result HasValue(RecordEnum<TEnum,TMember> value)
    {
        if (!Values.Contains(value))
        {
            return Result.Fail(NotAMemberMessage(value));
        }
        return Result.Succeed();
    }

    private static string NotAMemberMessage(RecordEnum<TEnum,TMember>? value) =>
        $"\"{value}\" is not a valid member of RecordEnum {typeof(TEnum)}. Valid members: {string.Join(", ", Values)}";
    
}
