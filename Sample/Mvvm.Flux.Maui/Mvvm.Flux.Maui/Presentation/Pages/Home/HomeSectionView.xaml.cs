using Mvvm.Flux.Maui.Presentation.CustomViews;
using Sharpnado.Maui.DragDropGridView;
using Sharpnado.TaskLoaderView;

namespace Mvvm.Flux.Maui.Presentation.Pages.Home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeSectionView : ContentPageView
    {
        public HomeSectionView()
        {
            InitializeComponent();
            TaskLoader.ResultControlTemplateLoaded += TaskLoaderOnResultControlTemplateLoaded;
        }

        private void TaskLoaderOnResultControlTemplateLoaded(object? sender, ControlTemplateLoadedEventArgs e)
        {
            var gridView = e.View.FindByName<DragDropGridView>("GridView");
            // gridView.DragAndDropItemsAnimation = DragDropAnimations.Items.WobbleAsync;
            // gridView.DragAndDropEndItemsAnimation = DragDropAnimations.EndItems.StopWobbleAsync;

            gridView.LongPressedDraggingAnimation = DragDropAnimations.Dragging.ScaleUpAsync;
            gridView.LongPressedDroppingAnimation = DragDropAnimations.Dropping.ScaleToNormalAsync;
        }
    }
}
