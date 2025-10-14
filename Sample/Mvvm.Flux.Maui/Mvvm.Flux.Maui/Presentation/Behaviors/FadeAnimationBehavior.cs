// https://github.com/josueyeray/XFBehaviors
// Latest commit 1e64dd2  on 28 Jan 2016

namespace Mvvm.Flux.Maui.Presentation.Behaviors
{
    public class FadeAnimationBehavior : AnimationBaseBehavior
    {
        public static readonly BindableProperty FinalOpacityProperty =
            BindableProperty.Create(nameof(FinalOpacity), typeof(double), typeof(FadeAnimationBehavior), 1.0);

        /// <summary>
        /// Final opacity, default: 1.
        /// </summary>
        public double FinalOpacity
        {
            get => (double)GetValue(FinalOpacityProperty);
            set => SetValue(FinalOpacityProperty, value);
        }

        protected override async Task DoAnimationAsync(View element, bool doNotAnimate = false)
        {
            await element.FadeTo(FinalOpacity, doNotAnimate ? 0 : (uint)Duration, GetEasingMethodFromEnumerator());
        }
    }
}