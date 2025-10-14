namespace Sharpnado.GridLayout;

public static class Computation
    {
        public static double Interpolate(double fromValue, double toValue, double t) =>
            fromValue + t * (toValue - fromValue);

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

        public static double SquareDistance(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        public static float ToRadians(float degrees)
        {
            return (float)((Math.PI / 180) * degrees);
        }
    }
