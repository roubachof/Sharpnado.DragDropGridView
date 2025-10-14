using Android.Content;
using Android.Views;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Sharpnado.GridLayout;

public class UntouchableRefreshViewHandler : RefreshViewHandler
{
    public void UpdateDisableScrolling(bool disableScrolling)
    {
        ((UntouchableMauiSwipeRefreshLayout)PlatformView).UodateDisableScrolling(disableScrolling);
    }

    protected override UntouchableMauiSwipeRefreshLayout CreatePlatformView()
    {
        var swipeRefreshLayout = new UntouchableMauiSwipeRefreshLayout(Context);

        return swipeRefreshLayout;
    }
}

public class UntouchableMauiSwipeRefreshLayout : MauiSwipeRefreshLayout
{
    private bool _disableScrolling;

    public UntouchableMauiSwipeRefreshLayout(Context context)
        : base(context)
    {
    }

    public void UodateDisableScrolling(bool disableScrolling)
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
