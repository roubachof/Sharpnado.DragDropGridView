namespace Sharpnado.Maui.DragDropGridView;

public static class GridAnimations
    {
        private static readonly GridLength ZeroHeightGrid = new GridLength(0);

        public static Task<bool> AnimateRowHeightAsync(
            this Grid view,
            RowDefinition rowDefinition,
            double rowExpandedHeight,
            bool isVisible,
            string animationName,
            bool dontAnimate = false)
        {
            const double tolerance = 2f;
            var tcs = new TaskCompletionSource<bool>();
            Animation animation;

            if (view.AbortAnimation(animationName))
            {
            }

            if (isVisible)
            {
                if (dontAnimate)
                {
                    rowDefinition.Height = rowExpandedHeight;
                    tcs.SetResult(true);
                    return tcs.Task;
                }

                if (rowDefinition.Height.Value > (rowExpandedHeight - tolerance))
                {
                    tcs.SetResult(false);
                    return tcs.Task;
                }

                // Move back to original height
                animation = new Animation(
                    d => rowDefinition.Height = new GridLength(Computation.Clamp(d, 0, double.MaxValue)),
                    rowDefinition.Height.Value,
                    rowExpandedHeight,
                    Easing.Linear,
                    () => animation = null);
            }
            else
            {
                if (dontAnimate)
                {
                    rowDefinition.Height = ZeroHeightGrid;
                    tcs.SetResult(true);
                    return tcs.Task;
                }

                if (Math.Abs(rowDefinition.Height.Value) < tolerance)
                {
                    tcs.SetResult(false);
                    return tcs.Task;
                }

                // Hide the row
                animation = new Animation(
                    d => rowDefinition.Height = new GridLength(Computation.Clamp(d, 0, double.MaxValue)),
                    rowExpandedHeight,
                    0,
                    Easing.Linear,
                    () => animation = null);
            }

            try
            {
                view.Animate(
                    animationName,
                    animation,
                    length: 400,
                    finished: (value, success) => tcs.TrySetResult(success));
            }
            catch (Exception)
            {
                tcs.SetResult(false);
            }

            return tcs.Task;
        }
    }
