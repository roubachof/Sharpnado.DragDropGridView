using MetroLog;
using MetroLog.Operators;
using MetroLog.Targets;

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Mopups.Hosting;

using Mvvm.Flux.Maui.Domain.Lights;
using Mvvm.Flux.Maui.Domain.Lights.Mock;
using Mvvm.Flux.Maui.Presentation.Pages;
using Mvvm.Flux.Maui.Presentation.Pages.Home;
using Sharpnado.GridLayout;
using Sharpnado.Shades;
using Sharpnado.Tabs;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LoggerFactory = MetroLog.LoggerFactory;
using LogLevel = MetroLog.LogLevel;

namespace Mvvm.Flux.Maui
{
    public static class MauiProgram
    {
        private static ILogger log;

        public static MauiApp CreateMauiApp()
        {
            Initialize();

            return MauiApp.CreateBuilder()
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureTaskLoader(true, true)
                .UseSharpnadoTabs(true)
                .UseSharpnadoShadows(false, false)
                .UseSharpnadoDragDropGridView(enableLogging: true, enableDebugLogging: true)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "FontRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "FontSemiBold");
                    fonts.AddFont("OpenSans-Bold.ttf", "FontBold");
                    fonts.AddFont("OpenSans-ExtraBold.ttf", "FontExtraBold");
                    fonts.AddFont("fa_5_pro_solid.otf", "FontAwesome");
                    fonts.AddFont("fa_5_pro_regular.otf", "FontAwesomeRegular");
                })
                .ConfigureMopups()
                .UsePrism(prism =>
                {
                    prism.RegisterTypes(container =>
                    {
                        RegisterDomain(container);
                        RegisterNavigation(container);
                    });


                    prism.CreateWindow(async navigationService =>
                    {
                        var navResult =
                            await navigationService.NavigateAsync(
                                nameof(NavigationPage) + "/" + nameof(MainPage));

                        if (navResult.Exception != null)
                        {
                            log.Error("Error while navigating", navResult.Exception!);
                        }
                    });
                })
                .Build();
        }

        private static void Initialize()
        {
            InitializeCrashService();

            log = InitializeLogger();
        }

        private static void RegisterDomain(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILightService, LightServiceMock>();
        }

        private static void RegisterNavigation(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<LightEditPage, LightEditPageViewModel>();
        }

        private static void InitializeCrashService()
        {
#if RELEASE
            if (PlatformService.IsEmulator)
            {
                return;
            }

            if (LogOperatorRetriever.Instance.TryGetOperator<ILogCompressor>(out var logCompressor))
            {
                // Attach logs to crash
            }
#endif
        }

        private static ILogger InitializeLogger()
        {
            var config = new LoggingConfiguration();

#if RELEASE
            // Will be stored in: $"MetroLog{Path.DirectorySeparatorChar}MetroLogs{Path.DirectorySeparatorChar}Log.log"
            if (!PlatformService.IsEmulator)
            {
                config.AddTarget(LogLevel.Info, LogLevel.Fatal, new StreamingFileTarget { RetainDays = 2 });
            }
            else
            {
                config.AddTarget(LogLevel.Debug, LogLevel.Fatal, new TraceTarget());
            }
#else
            config.AddTarget(LogLevel.Debug, LogLevel.Fatal, new TraceTarget());
#endif

            LoggerFactory.Initialize(config);

            var logger = LoggerFactory.GetLogger(nameof(App));

            TaskMonitorConfiguration.ErrorHandler = (t, m, e) => logger.Error(m, e);

            return logger;
        }
    }
}
