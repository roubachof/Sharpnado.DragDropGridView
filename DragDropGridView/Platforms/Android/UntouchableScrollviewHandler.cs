
using _Microsoft.Android.Resource.Designer;
using global::Android.Content;
using global::Android.Runtime;
using global::Android.Util;
using global::Android.Views;

using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;


namespace Sharpnado.Maui.DragDropGridView;

public class UntouchableScrollviewHandler : ScrollViewHandler
{
    public void UpdateDisableScrolling(bool disableScrolling)
    {
        ((UntouchableMauiScrollView)PlatformView).UpdateDisableScrolling(disableScrolling);
    }

    protected override MauiScrollView CreatePlatformView()
    {
        var scrollView = new UntouchableMauiScrollView(
            new ContextThemeWrapper(MauiContext!.Context, _Microsoft.Android.Resource.Designer.Resource.Style.scrollViewTheme),
            null!,
            _Microsoft.Android.Resource.Designer.Resource.Attribute.scrollViewStyle)
        {
            ClipToOutline = true,
            FillViewport = true,
        };

        return scrollView;
    }
}

public class UntouchableMauiScrollView : MauiScrollView
{
    private bool _disableScrolling;

    public UntouchableMauiScrollView(Context context)
        : base(context)
    {
    }

    public UntouchableMauiScrollView(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
    }

    public UntouchableMauiScrollView(Context context, IAttributeSet attrs, int defStyleAttr)
        : base(context, attrs, defStyleAttr)
    {
    }

    protected UntouchableMauiScrollView(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }

    public void UpdateDisableScrolling(bool disableScrolling)
    {
        _disableScrolling = disableScrolling;
    }

    public override bool OnInterceptTouchEvent(MotionEvent? ev)
    {
        if (_disableScrolling)
        {
            return false;
        }

        return base.OnInterceptTouchEvent(ev);
    }

    public override bool OnTouchEvent(MotionEvent? ev)
    {
        if (_disableScrolling)
        {
            return false;
        }

        return base.OnTouchEvent(ev);
    }
}
