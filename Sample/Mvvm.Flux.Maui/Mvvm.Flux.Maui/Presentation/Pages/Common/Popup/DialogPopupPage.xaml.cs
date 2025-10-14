using MetroLog;

using Mopups.Pages;

using Mvvm.Flux.Maui.Presentation.Navigation;

namespace Mvvm.Flux.Maui.Presentation.Pages.Common.Popup
{
    public partial class DialogPopupPage : PopupPage
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(DialogPopupPage));

        public DialogPopupPage()
        {
            InitializeComponent();

            BackgroundColor = new Color(0, 0, 0, 122);
        }

        public DialogPopupPage(View innerView)
            : this()
        {
            MainLayout.Children.Remove(TextContent);

            // Replace text label by innerView
            MainLayout.Children.Add(innerView);

            Grid.SetRow(innerView, 1);
            Grid.SetColumn(innerView, 0);

            if (innerView.HeightRequest > 1)
            {
                MainLayout.RowDefinitions[1].Height = new GridLength(innerView.HeightRequest);
            }
        }

        protected override void OnBindingContextChanged()
        {
            Log.Info($"OnBindingContextChanged {BindingContext}");

            base.OnBindingContextChanged();

            if (BindingContext == null)
            {
                return;
            }

            if (BindingContext is ADialogPopupPageViewModel viewModel)
            {
                InitActionButtons(viewModel.Actions);
            }
        }

        protected override bool OnBackgroundClicked()
        {
            // Deactivate popup closing on background click.
            return false;
        }

        private void InitActionButtons(PopupActions actions)
        {
            switch (actions)
            {
                case PopupActions.None:
                case PopupActions.Ok:
                    CancelButton.IsVisible = false;
                    OkButton.IsVisible = true;
                    break;
                case PopupActions.OkCancel:
                    CancelButton.IsVisible = true;
                    OkButton.IsVisible = true;
                    break;
                case PopupActions.Cancel:
                    CancelButton.IsVisible = true;
                    OkButton.IsVisible = false;
                    break;

            }
        }
    }
}