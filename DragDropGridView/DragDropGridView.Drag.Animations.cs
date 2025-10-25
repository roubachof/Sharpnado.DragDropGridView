namespace Sharpnado.Maui.DragDropGridView;

public partial class DragDropGridView
{
    private Task ApplyAnimationToChildrenAsync()
    {
        _animationCts?.Cancel();
        _animationCts?.Dispose();
        _animationCts = new CancellationTokenSource();
        List<Task> animationTasks = new List<Task>();
        foreach (var child in Children)
        {
            if (child == _headerView)
            {
                continue;
            }

            animationTasks.Add(ApplyAnimationAsync((View)child, _animationCts.Token));
        }

        return Task.WhenAll(animationTasks);
    }

    private async Task ApplyAnimationAsync(View view, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await DragAndDropItemsAnimation(view);
        }

        if (DragAndDropEndItemsAnimation != null)
        {
            await DragAndDropEndItemsAnimation(view);
        }
    }
}
