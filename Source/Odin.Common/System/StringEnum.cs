using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Odin.System;

/// <summary>
/// Provides enum-like behaviour for a set of string values.
/// Usage: Inherit from StringEnum and add ONLY public const string members.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public abstract class StringEnum<TEnum> where TEnum: StringEnum<TEnum> 
{
    // Static field in generic type is fine since we'll get a different one for each TEnum when StringEnum is reified.
    private static ImmutableHashSet<string>? _values = null;
    
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Lock ValuesReflectionLock = new Lock();

    /// <summary>
    /// Returns the set of string values
    /// </summary>
    public static IImmutableSet<string> Values
    {
        get
        {
            if (_values is not null)
            {
                return _values;
            }

            lock (ValuesReflectionLock)
            {
                if (_values is not null)
                {
                    return _values;
                }
                
                _values = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                    .Select(f => (string)f.GetValue(null)!)
                    .Distinct() // Allow duplicates
                    .ToImmutableHashSet();

                return _values;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static Outcome HasValue(string? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        StringComparer comparer = StringComparer.FromComparison(comparison);
        if (!Values.Contains(value, comparer))
        {
            return Outcome.Fail(NotAMemberMessage(value));
        }
        return Outcome.Succeed();
    }

    private static string NotAMemberMessage(string value) =>
        $"\"{value}\" is not a valid member of StringEnum {typeof(TEnum)}. Valid members: {string.Join(", ", Values)}";
    
}
