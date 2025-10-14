using System.ComponentModel;

namespace Mvvm.Flux.Maui.Presentation.Pages.Common.Validation
{
    public interface IValidatableField : INotifyPropertyChanged
    {
        List<string> ErrorMessages { get; }

        string FirstErrorMessage { get; }

        bool HasError { get; }

        bool Validate();
    }
}