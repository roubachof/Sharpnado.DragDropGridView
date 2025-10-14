using Mvvm.Flux.Maui.Presentation.Pages.Common.Popup;

namespace Mvvm.Flux.Maui.Presentation.Navigation
{
    public enum PopupActions
    {
        None = 0,
        Ok,
        OkCancel,
        Cancel,
    }

    public interface IPopupService
    {
        /// <summary>
        /// Pops the currently displayed popup asynchronously.
        /// </summary>
        Task PopAsync();

        void RegisterCustomPopup<TViewModel, TView>()
            where TView : View;

        /// <summary>
        /// Shows the progress for the provided task factory.
        /// </summary>
        /// <returns>The progress.</returns>
        /// <param name="taskFactory">Task factory.</param>
        /// <param name="progressText">Progress text.</param>
        void StartShowProgress(Func<Task> taskFactory, string progressText = null);

        /// <summary>
        /// Shows the progress for the provided task factory, accepting a parameter.
        /// </summary>
        /// <returns>The progress.</returns>
        /// <param name="taskFactory">Task factory.</param>
        /// <param name="parameter">Parameter to be used with the task factory.</param>
        /// <param name="progressText">Progress text.</param>
        /// <typeparam name="TParameter">Parameter type for the task factory.</typeparam>
        void StartShowProgress<TParameter>(Func<TParameter, Task> taskFactory, TParameter parameter, string progressText = null);

        Task AlertAsync(string title, string content, string confirmText = null);

        Task<bool> ConfirmAsync(string title, string content, string confirmText = null, string cancelText = null);

        /// <summary>
        /// Asks the user for a custom value of type TResult, using custom view and viewmodel.
        /// </summary>
        /// <returns>Async result of type T.</returns>
        /// <param name="customViewModel">Custom ViewModel used as a binding context for the custom popup.</param>
        /// <typeparam name="TResult">Expected result type.</typeparam>
        Task<TResult> AskCustomAsync<TResult>(IDialogPopupPageViewModel<TResult> customViewModel);
    }
}