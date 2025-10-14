namespace Mvvm.Flux.Maui.Infrastructure.Validation
{
    public interface IValidationRule
    {
        List<string> Validate(object value);
    }

    public abstract class AValidationRule<T> : IValidationRule
    {
        public List<string> Validate(object value)
        {
            return Validate((T)value);
        }

        protected abstract List<string> Validate(T value);
    }
}