// https://github.com/josueyeray/XFBehaviors
// Latest commit 1e64dd2  on 28 Jan 2016

using System.ComponentModel;

using Sharpnado.Tasks;

namespace Mvvm.Flux.Maui.Presentation.Behaviors
{
    /// <summary>
    /// Enumerator of event types our behavior could execute on raising.
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// View received focus.
        /// </summary>
        Focused,

        /// <summary>
        /// View lost focus.
        /// </summary>
        Unfocused,

        /// <summary>
        /// View add a child.
        /// </summary>
        ChildAdded,

        /// <summary>
        /// View remove a child.
        /// </summary>
        ChildRemoved,

        /// <summary>
        /// Behavior is attached to view.
        /// </summary>
        Attached,

        /// <summary>
        /// Behavior is detaching from view.
        /// </summary>
        Detaching,

        /// <summary>
        /// View becomes visible.
        /// </summary>
        IsVisible,

        /// <summary>
        /// View becomes invisible.
        /// </summary>
        IsNotVisible,
    }

    /// <summary>
    /// Enumerator with easing methods.
    /// </summary>
    public enum EasingMethodEnumerator
    {
        /// <summary>
        /// Jumps towards, and then bounces as it settles at the final value.
        /// </summary>
        BounceIn,

        /// <summary>
        /// Leaps to final values, bounces 3 times, and settles.
        /// </summary>
        BounceOut,

        /// <summary>
        /// Starts slowly and accelerates.
        /// </summary>
        CubicIn,

        /// <summary>
        /// Starts quickly and the decelerates.
        /// </summary>
        CubicOut,

        /// <summary>
        /// Accelerates and decelerates. Often a natural-looking choice.
        /// </summary>
        CubicInOut,

        /// <summary>
        /// Linear transformation.
        /// </summary>
        Linear,

        /// <summary>
        /// Smoothly accelerates.
        /// </summary>
        SinIn,

        /// <summary>
        /// Smoothly decelerates.
        /// </summary>
        SinOut,

        /// <summary>
        /// Accelerates in and out.
        /// </summary>
        SinInOut,

        /// <summary>
        /// Moves away and then leaps toward the final value.
        /// </summary>
        SpringIn,

        /// <summary>
        /// Overshoots and then returns.
        /// </summary>
        SpringOut,
    }

    public class AnimationBaseBehavior : BehaviorBase<View>
    {
        private static readonly BindableProperty DurationProperty =
            BindableProperty.Create(nameof(Duration), typeof(int), typeof(AnimationBaseBehavior), 250);

        private static readonly BindableProperty OnEventProperty =
            BindableProperty.Create(nameof(OnTrigger), typeof(TriggerType), typeof(AnimationBaseBehavior), TriggerType.Attached);

        private static readonly BindableProperty EasingMethodProperty =
            BindableProperty.Create(nameof(EasingMethod), typeof(EasingMethodEnumerator), typeof(AnimationBaseBehavior), EasingMethodEnumerator.Linear);

        /// <summary>
        /// Animation duration in milliseconds, default: 250ms
        /// </summary>
        public int Duration
        {
            get => (int)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        /// <summary>
        /// Launching event, default: Attached
        /// </summary>
        public TriggerType OnTrigger
        {
            get => (TriggerType)GetValue(OnEventProperty);
            set => SetValue(OnEventProperty, value);
        }

        /// <summary>
        /// Easing function to apply to animation, default: Linear
        /// </summary>
        public EasingMethodEnumerator EasingMethod
        {
            get => (EasingMethodEnumerator)GetValue(EasingMethodProperty);
            set => SetValue(EasingMethodProperty, value);
        }

        protected override void OnAttachedTo(View element)
        {
            base.OnAttachedTo(element);

            switch (OnTrigger)
            {
                case TriggerType.Focused:
                    element.Focused += ElementEvent;
                    break;

                case TriggerType.Unfocused:
                    element.Unfocused += ElementEvent;
                    break;

                case TriggerType.ChildAdded:
                    element.ChildAdded += ElementEvent;
                    break;

                case TriggerType.ChildRemoved:
                    element.ChildRemoved += ElementEvent;
                    break;

                case TriggerType.Attached:
                    DoAnimation(element);
                    break;

                case TriggerType.Detaching:
                    break;
            }
        }

        protected override void OnBindingContextChanged()
        {
            if (AssociatedObject.BindingContext != null)
            {
                switch (OnTrigger)
                {
                    case TriggerType.IsVisible:
                        if (AssociatedObject.IsVisible)
                        {
                            DoAnimation(AssociatedObject, doNotAnimate: true);
                        }

                        break;

                    case TriggerType.IsNotVisible:
                        if (!AssociatedObject.IsVisible)
                        {
                            DoAnimation(AssociatedObject, doNotAnimate: true);
                        }

                        break;
                }
            }
        }

        protected override void OnDetachingFrom(View element)
        {
            base.OnDetachingFrom(element);

            switch (OnTrigger)
            {
                case TriggerType.Focused:
                    element.Focused -= ElementEvent;
                    break;
                case TriggerType.Unfocused:
                    element.Unfocused -= ElementEvent;
                    break;
                case TriggerType.ChildAdded:
                    element.ChildAdded -= ElementEvent;
                    break;
                case TriggerType.ChildRemoved:
                    element.ChildRemoved -= ElementEvent;
                    break;
                case TriggerType.Attached:
                    break;
                case TriggerType.Detaching:
                    DoAnimation(element);
                    break;
                case TriggerType.IsVisible:
                case TriggerType.IsNotVisible:
                    break;
            }
        }

        protected override void OnAssociatedObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AssociatedObject.IsVisible)
                && ((OnTrigger == TriggerType.IsVisible && AssociatedObject.IsVisible)
                    || (OnTrigger == TriggerType.IsNotVisible && !AssociatedObject.IsVisible)))
            {
                DoAnimation(AssociatedObject);
            }
        }

        protected virtual Task DoAnimationAsync(View element, bool doNotAnimate = false)
        {
            return Task.CompletedTask;
        }

        protected Easing GetEasingMethodFromEnumerator()
        {
            switch (EasingMethod)
            {
                case EasingMethodEnumerator.BounceIn:
                    return Easing.BounceIn;
                case EasingMethodEnumerator.BounceOut:
                    return Easing.BounceOut;
                case EasingMethodEnumerator.CubicIn:
                    return Easing.CubicIn;
                case EasingMethodEnumerator.CubicOut:
                    return Easing.CubicOut;
                case EasingMethodEnumerator.CubicInOut:
                    return Easing.CubicInOut;
                case EasingMethodEnumerator.Linear:
                    return Easing.Linear;
                case EasingMethodEnumerator.SinIn:
                    return Easing.SinIn;
                case EasingMethodEnumerator.SinOut:
                    return Easing.SinOut;
                case EasingMethodEnumerator.SinInOut:
                    return Easing.SinInOut;
                case EasingMethodEnumerator.SpringIn:
                    return Easing.SpringIn;
                case EasingMethodEnumerator.SpringOut:
                    return Easing.SpringOut;
                default:
                    return Easing.Linear;
            }
        }

        private void DoAnimation(View element, bool doNotAnimate = false)
        {
            TaskMonitor.Create(DoAnimationAsync(element, doNotAnimate));
        }

        private void ElementEvent(object sender, EventArgs e)
        {
            DoAnimation((View)sender);
        }
    }
}
