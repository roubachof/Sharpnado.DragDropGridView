using System.ComponentModel;

namespace Mvvm.Flux.Maui.Presentation.Pages.Common.Popup
{
    /// <summary>
    /// Interface for popup view models that are intended to provide a return value,
    /// popup content being text only.
    /// </summary>
    public interface IDialogPopupPageViewModel<T>
    {
        Task<T> ResultTask { get; }

        T GetResult(bool isOk, object parameter);
    }

    /// <summary>
    /// Interface for popup view models that are intended to provide a return value,
    /// popup content being a custom view and an inner view model.
    /// </summary>
    public interface ICustomInnerDialogPopupPageViewModel<T> : IDialogPopupPageViewModel<T>
    {
        INotifyPropertyChanged InnerViewBindingContext { get; }
    }
}