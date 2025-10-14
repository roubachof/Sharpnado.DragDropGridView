using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Sharpnado.Tasks;

namespace Mvvm.Flux.Maui.Presentation.CustomViews
{
    public class FabMenu : Grid
    {
        public static readonly BindableProperty FoldButtonProperty = BindableProperty.Create(
            nameof(FoldButton),
            typeof(ImageButton),
            typeof(FabMenu),
            propertyChanged: (bindable, _, _) => ((FabMenu)bindable).Setup());

        public static readonly BindableProperty FoldedViewsProperty = BindableProperty.Create(
            nameof(FoldedViews),
            typeof(ObservableCollection<View>),
            typeof(FabMenu),
            defaultValueCreator: _ => new ObservableCollection<View>());

        private const double ItemSpacing = 10;

        private readonly ICommand _openTappedCommand;
        private bool _isSetup;
        private bool _isFolded = true;

        public FabMenu()
        {
            IsClippedToBounds = false;
            _openTappedCommand = new Command(OpenTapped);
            VerticalOptions = LayoutOptions.End;
            HorizontalOptions = LayoutOptions.End;

            FoldedViews.CollectionChanged += OnFoldedViewsCollectionChanged;
        }

        public ImageButton FoldButton
        {
            get => (ImageButton)GetValue(FoldButtonProperty);
            set => SetValue(FoldButtonProperty, value);
        }

        public ObservableCollection<View> FoldedViews
        {
            get => (ObservableCollection<View>)GetValue(FoldedViewsProperty);
            set => SetValue(FoldedViewsProperty, value);
        }

        public void Toggle()
        {
            if (!_isSetup || !FoldedViews.Any())
            {
                return;
            }

            if (_isFolded)
            {
                TaskMonitor.Create(Unfold);
                return;
            }

            TaskMonitor.Create(Fold());
        }

        private void Setup()
        {
            _isSetup = true;
            FoldButton.VerticalOptions = LayoutOptions.End;
            FoldButton.HorizontalOptions = LayoutOptions.End;
            FoldButton.Command = _openTappedCommand;
            FoldButton.ZIndex = 100;

            Children.Add(FoldButton);
        }

        private void OnFoldedViewsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (View view in e.NewItems!)
                    {
                        AddFoldedView(view);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                default:
                    throw new NotSupportedException();
            }
        }

        private void AddFoldedView(View view)
        {
            view.VerticalOptions = LayoutOptions.End;
            view.Opacity = 0;
            view.IsVisible = false;

            Children.Add(view);
        }

        private void OpenTapped(object obj)
        {
            Toggle();
        }

        private async Task Unfold()
        {
            List<Task> animations = [FoldButton.RotateTo(225)];

            int count = FoldedViews.Count;
            double heightRequest = -1;

            foreach (View foldedView in FoldedViews)
            {
                foldedView.IsVisible = true;
                double y = (count * ItemSpacing)
                    + ((count - 1) * foldedView.HeightRequest)
                    + FoldButton.Height;

                count--;
                if (heightRequest < 0)
                {
                    heightRequest = y + foldedView.HeightRequest;
                }

                animations.Add(
                    Task.WhenAll(
                        foldedView.FadeTo(1),
                        foldedView.TranslateTo(0, -y)));
            }

            _isFolded = false;

            await Task.WhenAll(animations);
            HeightRequest = heightRequest;
        }

        private async Task Fold()
        {
            List<Task> animations = [FoldButton.RotateTo(0)];

            static async Task FoldView(View view)
            {
                await Task.WhenAll(
                    view.FadeTo(0),
                    view.TranslateTo(0, 0));
                view.IsVisible = false;
            }

            foreach (View foldedView in FoldedViews!)
            {
                animations.Add(FoldView(foldedView));
            }

            _isFolded = true;

            await Task.WhenAll(animations);
            HeightRequest = FoldButton.Height;
        }
    }
}