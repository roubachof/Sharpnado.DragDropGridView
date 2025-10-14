using MetroLog;

namespace Mvvm.Flux.Maui.Infrastructure.Logging
{
    public static class LoggingExtensions
    {
        public static async Task<TData> InfoListResultAsync<TData>(this ILogger logger, Task<TData> resultTask, string itemType = "items")
            where TData : IReadOnlyCollection<object>
        {
            var result = await resultTask;
            if (result == null)
            {
                logger.Info("returning null");
                return default(TData);
            }

            logger.Info($"returning {result.Count} {itemType}");
            return result;
        }

        public static async Task<TData> InfoResultAsync<TData>(this ILogger logger, Task<TData> resultTask, string itemType = "items")
            where TData : class
        {
            var result = await resultTask;
            if (result == null)
            {
                logger.Info("returning null");
                return default(TData);
            }

            logger.Info("returning a non null result");
            return result;
        }
    }
}
