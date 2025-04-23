namespace Odin.Utility.VaryingValues;

/// <summary>
/// Supports the returning of a Value that may change based on another varying Dimension (eg DateOnly)
/// </summary>
/// <typeparam name="TVaryingDimensionType"></typeparam>
/// <typeparam name="TValueType"></typeparam>
public interface IVaryingValueProvider<in TVaryingDimensionType, out TValueType> where TVaryingDimensionType : IComparable 
{
    /// <summary>
    /// Gets the Value
    /// </summary>
    /// <param name="atPointAlongDimension"></param>
    /// <returns></returns>
    TValueType? GetValue(TVaryingDimensionType atPointAlongDimension);
}