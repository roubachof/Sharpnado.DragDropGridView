namespace Sharpnado.Maui.DragDropGridView;

/// <summary>
/// Predefined animations for drag and drop grid view items.
/// </summary>
public static class DragDropAnimations
{
    /// <summary>
    /// Animations that can be applied when drag and drop mode is enabled.
    /// </summary>
    public static class Items
    {
        /// <summary>
        /// Wobble animation that continuously rotates the view back and forth.
        /// </summary>
        public static Task WobbleAsync(View view) => view.Wobble();

        /// <summary>
        /// Cleanup animation to reset rotation after wobble.
        /// </summary>
        public static Task StopWobbleAsync(View view) => view.RotateTo(0, 100);

        /// <summary>
        /// No animation - items remain static.
        /// </summary>
        public static Task NoneAsync(View view) => Task.CompletedTask;
    }

    /// <summary>
    /// Animations that can be applied when drag and drop mode is disabled.
    /// </summary>
    public static class EndItems
    {
        /// <summary>
        /// Cleanup animation to reset rotation after wobble.
        /// </summary>
        public static Task StopWobbleAsync(View view) => view.RotateTo(0, 100);

        /// <summary>
        /// No animation - items remain static.
        /// </summary>
        public static Task NoneAsync(View view) => Task.CompletedTask;
    }

    /// <summary>
    /// Animations that can be applied when a view starts being dragged.
    /// </summary>
    public static class Dragging
    {
        /// <summary>
        /// Scale up animation to indicate the view is being picked up.
        /// </summary>
        public static Task ScaleUpAsync(View view) => view.ScaleTo(1.05, 100);

        /// <summary>
        /// More pronounced scale up animation.
        /// </summary>
        public static Task ScaleUpLargeAsync(View view) => view.ScaleTo(1.15, 150);

        /// <summary>
        /// Scale up with a bounce effect.
        /// </summary>
        public static async Task ScaleUpBounceAsync(View view)
        {
            await view.ScaleTo(1.2, 100);
            await view.ScaleTo(1.05, 100);
        }

        /// <summary>
        /// No animation - view remains at original scale.
        /// </summary>
        public static Task NoneAsync(View view) => Task.CompletedTask;
    }

    /// <summary>
    /// Animations that can be applied when a view stops being dragged.
    /// </summary>
    public static class Dropping
    {
        /// <summary>
        /// Scale back to normal size.
        /// </summary>
        public static Task ScaleToNormalAsync(View view) => view.ScaleTo(1, 100);

        /// <summary>
        /// Scale back to normal with a bounce effect.
        /// </summary>
        public static async Task ScaleToBounceAsync(View view)
        {
            await view.ScaleTo(0.95, 100);
            await view.ScaleTo(1, 100);
        }

        /// <summary>
        /// No animation - view remains at current scale.
        /// </summary>
        public static Task NoneAsync(View view) => Task.CompletedTask;
    }
}
