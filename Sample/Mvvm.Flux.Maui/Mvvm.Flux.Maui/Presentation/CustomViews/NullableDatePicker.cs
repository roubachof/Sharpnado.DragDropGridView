// https://gist.githubusercontent.com/Char0394/485379e2d80789f4c1c20b0d0a80cb2c/raw/e7a5edfac361fe1c6cb1ed6200b54f2b107a1011/NullableDatePicker.cs

namespace Mvvm.Flux.Maui.Presentation.CustomViews
{
    public class NullableDatePicker : DatePicker
    {
        public static readonly BindableProperty PlaceHolderProperty = BindableProperty.Create(
            nameof(PlaceHolder),
            typeof(string),
            typeof(NullableDatePicker),
            "/ . / . /");

        public static readonly BindableProperty NullableDateProperty = BindableProperty.Create(
            nameof(NullableDate),
            typeof(DateTime?),
            typeof(NullableDatePicker),
            null,
            defaultBindingMode: BindingMode.TwoWay);

        public NullableDatePicker()
        {
            Format = "d";
        }

        public string OriginalFormat { get; private set; }

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }

        public DateTime? NullableDate
        {
            get => (DateTime?)GetValue(NullableDateProperty);
            set
            {
                SetValue(NullableDateProperty, value);
                UpdateDate();
            }
        }

        public void CleanDate()
        {
            NullableDate = null;
            UpdateDate();
        }

        public void AssignValue()
        {
            NullableDate = Date;
            UpdateDate();
        }


        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                OriginalFormat = Format;
                UpdateDate();
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == DateProperty.PropertyName
                || (propertyName == IsFocusedProperty.PropertyName
                    && !IsFocused
                    && (Date.ToString("d") == DateTime.Now.ToString("d"))))
            {
                AssignValue();
            }

            if (propertyName == NullableDateProperty.PropertyName && NullableDate.HasValue)
            {
                Date = NullableDate.Value;
                if (Date.ToString(OriginalFormat) == DateTime.Now.ToString(OriginalFormat))
                {
                    //this code was done because when date selected is the actual date the"DateProperty" does not raise
                    UpdateDate();
                }
            }
        }

        private void UpdateDate()
        {
            if (NullableDate != null)
            {
                if (OriginalFormat != null)
                {
                    Format = OriginalFormat;
                }
            }
            else
            {
                Format = PlaceHolder;
            }
        }
    }
}