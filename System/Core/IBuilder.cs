namespace Odin.System
{
    /// <summary>
    /// Standard factory pattern for complex object creation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuilder<out T> where T : class
    {
        /// <summary>
        /// Returns the actual entity being constructed..
        /// </summary>
        /// <returns></returns>
        T Build();
    }
}