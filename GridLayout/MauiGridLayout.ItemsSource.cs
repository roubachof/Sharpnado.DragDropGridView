namespace Sharpnado.GridLayout;

using System.Collections;
using System.Collections.Specialized;

public partial class GridLayout
{
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(GridLayout),
        default(IEnumerable),
        propertyChanged: OnItemsSourceChanged);

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(GridLayout),
        default(DataTemplate),
        propertyChanged: OnItemTemplateChanged);

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not GridLayout gridLayout)
        {
            return;
        }

        if (oldValue is INotifyCollectionChanged oldItemsSource)
        {
            oldItemsSource.CollectionChanged -= gridLayout.ItemsSourceCollectionChanged;
        }

        gridLayout.Clear();

        if (newValue is null)
        {
            return;
        }

        foreach (var item in (IEnumerable)newValue)
        {
            if (gridLayout.ItemTemplate.CreateContent() is not View view)
            {
                continue;
            }

            view.BindingContext = item;
            gridLayout.Children.Add(view);
        }

        if (newValue is INotifyCollectionChanged newItemsSource)
        {
            newItemsSource.CollectionChanged += gridLayout.ItemsSourceCollectionChanged;
        }
    }

    private static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not GridLayout gridLayout)
        {
            return;
        }

        gridLayout.Clear();

        if (gridLayout.ItemsSource is null)
        {
            return;
        }

        if (gridLayout.ItemsSource is INotifyCollectionChanged notifyCollection)
        {
            notifyCollection.CollectionChanged -= gridLayout.ItemsSourceCollectionChanged;
            notifyCollection.CollectionChanged += gridLayout.ItemsSourceCollectionChanged;
        }

        foreach (var item in gridLayout.ItemsSource)
        {
            if (gridLayout.ItemTemplate.CreateContent() is not View view)
            {
                continue;
            }

            view.BindingContext = item;
            gridLayout.Children.Add(view);
        }
    }

    private void ItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                Clear();

                break;

            case NotifyCollectionChangedAction.Add when e.NewItems is not null:
                foreach (var item in e.NewItems)
                {
                    if (ItemTemplate.CreateContent() is not View view)
                    {
                        continue;
                    }

                    view.BindingContext = item;
                    Children.Add(view);
                }

                break;

            case NotifyCollectionChangedAction.Remove when e.OldItems is not null:
                foreach (var item in e.OldItems)
                {
                    if (Children.FirstOrDefault(c => ((View)c).BindingContext == item) is not View view)
                    {
                        continue;
                    }

                    Children.Remove(view);
                }

                break;

            case NotifyCollectionChangedAction.Replace when e.NewItems is not null && e.OldItems is not null:
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var view = Children.FirstOrDefault(c => ((View)c).BindingContext == e.OldItems[i]);
                    if (view is null)
                    {
                        continue;
                    }

                    ((View)view).BindingContext = e.NewItems[i];
                }

                break;

            case NotifyCollectionChangedAction.Move when e.NewItems is not null && e.OldItems is not null:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}