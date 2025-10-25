namespace Sharpnado.Maui.DragDropGridView;

public static class AnimationFactory
    {
        public static Animation GetGrowAnimation(VisualElement visualElement)
        {
            var animation = new Animation();

            animation.WithConcurrent(f => visualElement.Scale = f, 1.20, 1, Easing.BounceIn);

            return animation;
        }
    }
