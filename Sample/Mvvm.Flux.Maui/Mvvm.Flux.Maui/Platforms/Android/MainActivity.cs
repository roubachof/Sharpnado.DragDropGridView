using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Mvvm.Flux.Maui.Infrastructure;

using Color = Android.Graphics.Color;

namespace Mvvm.Flux.Maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            PlatformService.Initialize(
                IsRunningInEmulator(),
                Resources.DisplayMetrics.Density,
                (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density),
                (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density),
                GetSafeArea);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                Window.SetFlags(WindowManagerFlags.TranslucentStatus, WindowManagerFlags.TranslucentStatus);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutFullscreen);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.SetStatusBarColor(Color.Transparent);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.LollipopMr1)
            {
                Window.DecorView.SystemUiVisibility =
                    (StatusBarVisibility)((SystemUiFlags)Window.DecorView.SystemUiVisibility
                                          | SystemUiFlags.LightStatusBar);
            }
        }

        private static bool IsRunningInEmulator()
        {
            return (Build.Brand.StartsWith("generic") && Build.Device.StartsWith("generic"))
                   || Build.Fingerprint.StartsWith("generic")
                   || Build.Fingerprint.StartsWith("unknown")
                   || Build.Hardware.Contains("goldfish")
                   || Build.Hardware.Contains("ranchu")
                   || Build.Model.Contains("google_sdk")
                   || Build.Model.Contains("Emulator")
                   || Build.Model.Contains("Android SDK built for x86")
                   || Build.Manufacturer.Contains("Genymotion")
                   || Build.Product.Contains("sdk_google")
                   || Build.Product.Contains("google_sdk")
                   || Build.Product.Contains("sdk")
                   || Build.Product.Contains("sdk_x86")
                   || Build.Product.Contains("vbox86p")
                   || Build.Product.Contains("emulator")
                   || Build.Product.Contains("simulator");
        }

        private Thickness GetSafeArea()
        {
            var defaultArea = new Thickness(0, 24, 0, 0);
            if (Build.VERSION.SdkInt < BuildVersionCodes.P)
            {
                return defaultArea;
            }

            var cutout = Window?.DecorView.RootWindowInsets?.DisplayCutout;
            if (cutout == null)
            {
                return defaultArea;
            }

            return new Thickness(
                cutout.SafeInsetLeft / Resources.DisplayMetrics.Density,
                (cutout.SafeInsetTop / Resources.DisplayMetrics.Density),
                cutout.SafeInsetRight / Resources.DisplayMetrics.Density,
                cutout.SafeInsetBottom / Resources.DisplayMetrics.Density);
        }
    }
}
