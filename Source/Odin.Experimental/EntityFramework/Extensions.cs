using Microsoft.EntityFrameworkCore;
 using Microsoft.EntityFrameworkCore.Metadata.Builders;

 namespace Odin.EntityFramework
{
    /// <summary>
    /// EF extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// For SQl Server decimal columns
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static PropertyBuilder<decimal?> HasPrecision(this PropertyBuilder<decimal?> builder, int precision, int scale)
        {
            return builder.HasColumnType($"decimal({precision},{scale})");
        }

        /// <summary>
        /// For SQl Server decimal columns
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static PropertyBuilder<decimal> HasPrecision(this PropertyBuilder<decimal> builder, int precision, int scale)
        {
            return builder.HasColumnType($"decimal({precision},{scale})");
        }
    }
}
