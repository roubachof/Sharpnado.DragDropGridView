using Mvvm.Flux.Maui.Localization;

namespace Mvvm.Flux.Maui.Infrastructure
{
    public static class ApplicationExceptions
    {
        public static string ToString(Exception exception)
        {
            return exception switch
            {
                ServerException => GlobalResources.Error_Server_Technical,
                NetworkException => GlobalResources.Error_Network,
                LocalizableException localizableException => localizableException.Message,
                _ => GlobalResources.Error_Unhandled,
            };
        }
    }

    public class LocalizableException : Exception
    {
    }

    public class ServerException : Exception
    {
    }

    public class NetworkException : Exception
    {
    }
}
