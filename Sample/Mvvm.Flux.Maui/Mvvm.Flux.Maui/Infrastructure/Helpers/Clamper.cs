namespace Mvvm.Flux.Maui.Infrastructure.Helpers
{
    public static class Clamper
    {
        public static int Clamp(int value, int minValue, int maxValue)
        {
            if (value > maxValue)
            {
                return maxValue;
            }

            if (value < minValue)
            {
                return minValue;
            }

            return value;
        }

        public static double Clamp(double value, double minValue, double maxValue)
        {
            if (value > maxValue)
            {
                return maxValue;
            }

            if (value < minValue)
            {
                return minValue;
            }

            return value;
        }
    }
}