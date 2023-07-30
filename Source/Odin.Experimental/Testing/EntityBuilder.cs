
namespace Odin.Testing
{
    /// <summary>
    /// Base pattern to use for object fabrication, particularly for creating test entities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityBuilder<T>
    {
        /// <summary>
        /// Entity in question
        /// </summary>
        protected T _entity;

        /// <summary>
        /// Returns the actual entity being constructed..
        /// </summary>
        /// <returns></returns>
        public T ToEntity()
        {
            return _entity;
        }
    }
}