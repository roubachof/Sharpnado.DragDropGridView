using Foundation;

using Mvvm.Flux.Maui.Infrastructure;

using ObjCRuntime;

using UIKit;

namespace Mvvm.Flux.Maui
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            PlatformService.Initialize(
                Runtime.Arch == Arch.SIMULATOR,
                UIScreen.MainScreen.Scale,
                (int)UIScreen.MainScreen.Bounds.Width,
                (int)UIScreen.MainScreen.Bounds.Height,
                SafeAreaGetter);

            return base.FinishedLaunching(app, options);
        }

        private static Thickness SafeAreaGetter()
        {
            UIEdgeInsets safeArea;

            if (!UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                safeArea = new UIEdgeInsets(UIApplication.SharedApplication.StatusBarFrame.Size.Height, 0, 0, 0);
            }
            else if (UIApplication.SharedApplication.KeyWindow != null)
            {
                safeArea = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;
            }
            else if (UIApplication.SharedApplication.Windows.Length > 0)
            {
                safeArea = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
            }
            else
            {
                safeArea = UIEdgeInsets.Zero;
            }

            return new Thickness(
                safeArea.Left,
                safeArea.Top,
                safeArea.Right,
                safeArea.Bottom > 0 ? safeArea.Bottom - 10 : safeArea.Bottom);
        }
    }
}
