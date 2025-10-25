namespace Sharpnado.Maui.DragDropGridView;

public static class ColorHelper
    {
        public static Color Interpolate(Color fromColor, Color toColor, double t)
            => Color.FromRgba(
                fromColor.Red + (t * (toColor.Red - fromColor.Red)),
                fromColor.Green + (t * (toColor.Green - fromColor.Green)),
                fromColor.Blue + (t * (toColor.Blue - fromColor.Blue)),
                fromColor.Alpha + (t * (toColor.Alpha - fromColor.Alpha)));
    }
