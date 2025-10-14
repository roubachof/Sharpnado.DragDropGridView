using MetroLog;

using Mopups.Interfaces;

using Mvvm.Flux.Maui.Presentation.Pages.Common.Popup;

namespace Mvvm.Flux.Maui.Presentation.Navigation.Impl
{
    public class PopupService : IPopupService
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(PopupService));

        private readonly IPopupNavigation _popupNavigation;

        private readonly IDictionary<Type, Type> _customViewTypes;

        public PopupService(IPopupNavigation popupNavigation)
        {
            _popupNavigation = popupNavigation;

            _customViewTypes = new Dictionary<Type, Type>();
        }

        public Task PopAsync() => _popupNavigation.PopAsync();

        public Task AlertAsync(
            string title,
            string content,
            string confirmText = null)
        {
            Log.Info($"AlertAsync( title: {title}, content: {content}, confirmText {confirmText} )");

            var alertViewModel =
                new AlertDialogPopupPageViewModel(
                    this,
                    title,
                    content,
                    confirmText);

            var popupPage = new DialogPopupPage { BindingContext = alertViewModel };
            return _popupNavigation.PushAsync(popupPage);
        }

        public async Task<bool> ConfirmAsync(
            string title,
            string content,
            string confirmText = null,
            string cancelText = null)
        {
            Log.Info($"ConfirmAsync( title: {title}, content: {content}, confirmText: {confirmText}, cancelText: {cancelText} )");

            var confirmationViewModel =
                new ConfirmDialogPageViewModel(
                    this,
                    title,
                    content,
                    confirmText,
                    cancelText);

            var popupPage = new DialogPopupPage
            {
                BindingContext = confirmationViewModel,
            };

            await _popupNavigation.PushAsync(popupPage);

            return await confirmationViewModel.ResultTask;
        }

        public async Task<TResult> AskCustomAsync<TResult>(IDialogPopupPageViewModel<TResult> customViewModel)
        {
            Log.Info($"AskCustomAsync()");

            // First, create inner view and set binding context
            var innerView = (View)Activator.CreateInstance(_customViewTypes[customViewModel.GetType()]);
            if (customViewModel is ICustomInnerDialogPopupPageViewModel<TResult> customInnerDialogViewModel)
            {
                innerView.BindingContext = customInnerDialogViewModel.InnerViewBindingContext;
            }

            // Then create DialogPopupPage and set binding context
            var popupPage = new DialogPopupPage(innerView)
            {
                BindingContext = customViewModel,
            };

            await _popupNavigation.PushAsync(popupPage);

            return await customViewModel.ResultTask;
        }

        public void RegisterCustomPopup<TViewModel, TView>()
            where TView : View
        {
            if (_customViewTypes.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Error: already registered {typeof(TViewModel)}.");
            }

            _customViewTypes.Add(typeof(TViewModel), typeof(TView));
        }

        public void StartShowProgress(Func<Task> taskFactory, string progressText = null)
        {
            throw new NotImplementedException();
        }

        public void StartShowProgress<TParameter>(Func<TParameter, Task> taskFactory, TParameter parameter, string progressText = null)
        {
            throw new NotImplementedException();
        }
    }
}