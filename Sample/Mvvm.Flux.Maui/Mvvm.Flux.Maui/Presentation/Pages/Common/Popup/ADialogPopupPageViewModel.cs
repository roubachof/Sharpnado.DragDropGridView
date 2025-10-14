using System.Windows.Input;

using Mvvm.Flux.Maui.Localization;
using Mvvm.Flux.Maui.Presentation.Navigation;

using Sharpnado.TaskLoaderView;

namespace Mvvm.Flux.Maui.Presentation.Pages.Common.Popup
{
    public abstract class ADialogPopupPageViewModel : BindableBase
    {
        private readonly IPopupService _popupService;

        private string _title;
        private string _contentText;

        private string _okText;
        private string _cancelText;

        private PopupActions _actions;

        protected ADialogPopupPageViewModel(
            IPopupService popupService,
            string title,
            string contentText = null,
            string confirmText = null,
            string cancelText = null,
            PopupActions actions = PopupActions.Ok)
        {
            _popupService = popupService;

            Actions = actions;

            Title = title;
            ContentText = contentText;

            OkText = string.IsNullOrWhiteSpace(confirmText) ? GlobalResources.Common_Ok : confirmText;
            CancelText = string.IsNullOrWhiteSpace(cancelText) ? GlobalResources.Common_Cancel : cancelText;

            OkCommand = new TaskLoaderCommand<object>(DoOk);
            CancelCommand = new TaskLoaderCommand(DoCancel);
        }

        public string Title
        {
            get => _title.ToUpper();
            set => SetProperty(ref _title, value);
        }

        public string ContentText
        {
            get => _contentText;
            set => SetProperty(ref _contentText, value);
        }

        public string OkText
        {
            get => _okText.ToUpper();
            set => SetProperty(ref _okText, value);
        }

        public string CancelText
        {
            get => _cancelText.ToUpper();
            set => SetProperty(ref _cancelText, value);
        }

        public PopupActions Actions
        {
            get => _actions;
            set => SetProperty(ref _actions, value);
        }

        public bool IsSuccessful { get; private set; }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        protected virtual async Task DoCancel()
        {
            await _popupService.PopAsync();
        }

        protected virtual async Task DoOk(object parameter)
        {
            IsSuccessful = true;
            await _popupService.PopAsync();
        }
    }

    public abstract class ADialogPopupPageViewModel<T> : ADialogPopupPageViewModel, IDialogPopupPageViewModel<T>
    {
        protected ADialogPopupPageViewModel(
            IPopupService popupService,
            string title,
            string contentText = null,
            string confirmText = null,
            string cancelText = null,
            PopupActions actions = PopupActions.Ok)
            : base(popupService, title, contentText, confirmText, cancelText, actions)
        {
            ResultTaskSource = new TaskCompletionSource<T>();
        }

        public Task<T> ResultTask => ResultTaskSource.Task;

        protected TaskCompletionSource<T> ResultTaskSource { get; }

        public abstract T GetResult(bool isOk, object parameter);

        protected override async Task DoCancel()
        {
            await base.DoCancel();
            ResultTaskSource.SetResult(GetResult(false, false));
        }

        protected override async Task DoOk(object parameter)
        {
            await base.DoOk(parameter);
            ResultTaskSource.SetResult(GetResult(true, parameter));
        }
    }
}