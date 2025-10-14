using Mvvm.Flux.Maui.Infrastructure.Validation;

namespace Mvvm.Flux.Maui.Presentation.Pages.Common.Validation
{
    public class ValidatableField<T> : BindableBase, IValidatableField
    {
        private bool _hasError;
        private List<string> _errorMessages;
        private T _value;
        private bool _isFirstSet = true;

        public ValidatableField(params IValidationRule[] rules)
        {
            Rules = rules;
            ErrorMessages = new List<string>();
        }

        public T Value
        {
            get => _value;
            set
            {
                if (_isFirstSet && (value is null || value.Equals(default(T))))
                {
                    Validate();
                }
                else if (SetProperty(ref _value, value))
                {
                    Validate();
                }

                _isFirstSet = false;
            }
        }

        public List<string> ErrorMessages
        {
            get => _errorMessages;
            set => SetProperty(ref _errorMessages, value);
        }

        public string FirstErrorMessage => ErrorMessages.FirstOrDefault();

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        protected IValidationRule[] Rules { get; }

        public virtual bool Validate()
        {
            List<string> validationResult = new List<string>();
            foreach (var validationRule in Rules)
            {
                validationResult.AddRange(validationRule.Validate(Value));
            }

            ErrorMessages = validationResult;
            HasError = ErrorMessages.Any();
            RaisePropertyChanged(nameof(FirstErrorMessage));

            return !HasError;
        }
    }
}