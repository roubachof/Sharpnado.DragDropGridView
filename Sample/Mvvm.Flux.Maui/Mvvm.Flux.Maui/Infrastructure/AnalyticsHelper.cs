namespace Mvvm.Flux.Maui.Infrastructure
{
    public static class AnalyticsHelper
    {
        public const string ScreenDisplayed = nameof(ScreenDisplayed);
        public const string LifecycleStateChanged = nameof(LifecycleStateChanged);
        public const string MenuRevealed = nameof(MenuRevealed);

        public const string DisplayedScreen = nameof(DisplayedScreen);
        public const string LifecycleState = nameof(LifecycleState);

        public const string StartState = "Start";
        public const string ResumeState = "Resume";
        public const string SleepState = "Sleep";

        public static void TrackScreenDisplayed(string screenName)
        {
            // Analytics.TrackEvent(ScreenDisplayed, new Dictionary<string, string> { { DisplayedScreen, screenName } });
        }

        public static void TrackLifecycleStateChanged(string stateName)
        {
            // Analytics.TrackEvent(LifecycleStateChanged, new Dictionary<string, string> { { LifecycleState, stateName } });
        }

        public static void TrackMenuRevealed()
        {
            // Analytics.TrackEvent(MenuRevealed);
        }
    }
}
