namespace Mvvm.Flux.Maui.Infrastructure.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                return new List<T>();
            }

            return list;
        }

        public static IEnumerable<T> DefaultIfNull<T>(this IEnumerable<T> list, T defaultValue)
        {
            if (list == null)
            {
                return new List<T> { defaultValue };
            }

            return list;
        }
    }
}