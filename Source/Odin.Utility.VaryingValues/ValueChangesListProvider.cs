using Microsoft.Extensions.Configuration;
using Odin.DesignContracts;

namespace Odin.Utility.VaryingValues;

public class ValueChangesListProvider<TRangeType, TValueType> : IVaryingValueProvider<TRangeType, TValueType> 
    where TRangeType : IComparable
{
    internal List<ValueChange<TRangeType, TValueType>> _valueChangesInOrder = null!;
    
    public ValueChangesListProvider(IEnumerable<ValueChange<TRangeType, TValueType>> valuesAcrossRange)
    {
        PreCondition.RequiresNotNull(valuesAcrossRange);
        InitialiseFrom(valuesAcrossRange.ToList());
    }

    public ValueChangesListProvider(IConfiguration configuration, string sectionName)
    {
        PreCondition.RequiresNotNull(configuration);
        List<ValueChange<TRangeType, TValueType>> valuesInConfig = new List<ValueChange<TRangeType, TValueType>>();
        configuration.Bind(sectionName, valuesInConfig);
        InitialiseFrom(valuesInConfig);
    }
    
    public ValueChangesListProvider(IConfigurationSection section)
    {
        PreCondition.RequiresNotNull(section);
        List<ValueChange<TRangeType, TValueType>> valuesInConfig = new List<ValueChange<TRangeType, TValueType>>();
        section.Bind(valuesInConfig);
        InitialiseFrom(valuesInConfig);
    }

    private void InitialiseFrom(List<ValueChange<TRangeType, TValueType>> valuesAcrossRange)
    {
        PreCondition.RequiresNotNull(valuesAcrossRange);
        _valueChangesInOrder = valuesAcrossRange.OrderBy(c => c.From).ToList();
    }


    public TValueType? GetValue(TRangeType atPointAlongDimension)
    {
        var candidates = _valueChangesInOrder.Where(c => c.From.CompareTo(atPointAlongDimension) <= 0);
        ValueChange<TRangeType, TValueType>? applicableValue = candidates.MaxBy(c => c.From);
        if (applicableValue == null)
        {
            return default;
        }
        return applicableValue.Value;
    }
}