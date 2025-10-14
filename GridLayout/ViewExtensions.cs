namespace Sharpnado.GridLayout;

public static class ViewExtensions
    {
        public static (double X, double Y) GetCenter(this VisualElement view)
        {
            return (view.X + view.Width / 2, view.Y + view.Height / 2);
        }

        public static (double X, double Y) GetTranslatingCenter(this VisualElement view)
        {
            return (view.X + view.TranslationX + (view.Width / 2), view.Y + view.TranslationY + (view.Height / 2));
        }

        public static List<T> GetAllChildrenOfType<T>(this Element view)
        {
            if (view is ContentPage page)
            {
                view = page.Content;
            }

            var output = new List<T>();

            if (view is T tview)
            {
                output.Add(tview);
            }

            if (view is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    if (child is Element elementChild)
                    {
                        output.AddRange(elementChild.GetAllChildrenOfType<T>());
                    }
                }
            }

            return output;
        }

        public static T GetFirstParentOfType<T>(this Element view)
        {
            var output = view;
            while (output.Parent != null)
            {
                if (output.Parent is T parent)
                {
                    return parent;
                }

                output = output.Parent;
            }

            return default;
        }

        /// <summary>
        ///     Gets the screen coordinates from top left corner.
        /// </summary>
        /// <returns>The screen coordinates.</returns>
        /// <param name="view">View.</param>
        public static (double X, double Y) GetScreenCoordinates<T>(this VisualElement view)
        {
            // A view's default X- and Y-coordinates are LOCAL with respect to the boundaries of its parent,
            // and NOT with respect to the screen. This method calculates the SCREEN coordinates of a view.
            // The coordinates returned refer to the top left corner of the view.

            // Initialize with the view's "local" coordinates with respect to its parent
            double screenCoordinateX = view.X;
            double screenCoordinateY = view.Y;

            // Get the view's parent (if it has one...)
            if (view.Parent.GetType() != typeof(T))
            {
                var parent = (VisualElement)view.Parent;

                // Loop through all parents
                while (parent != null)
                {
                    // Add in the coordinates of the parent with respect to ITS parent
                    screenCoordinateX += parent.X;
                    screenCoordinateY += parent.Y;

                    // If the parent of this parent isn't the app itself, get the parent's parent.
                    if (parent.Parent.GetType() == typeof(T))
                    {
                        parent = null;
                    }
                    else if (parent.Parent is VisualElement parentElement)
                    {
                        parent = parentElement;
                    }
                    else
                    {
                        parent = null;
                    }
                }
            }

            // Return the final coordinates...which are the global SCREEN coordinates of the view
            return (screenCoordinateX, screenCoordinateY);
        }

        /// <summary>
        ///     Gets the screen coordinates from top left corner (walks up to Page/Window).
        /// </summary>
        /// <returns>The screen coordinates.</returns>
        /// <param name="view">View.</param>
        public static (double X, double Y) GetScreenCoordinates(this VisualElement view)
        {
            double screenCoordinateX = view.X;
            double screenCoordinateY = view.Y;

            var parent = view.Parent as VisualElement;
            while (parent != null)
            {
                screenCoordinateX += parent.X;
                screenCoordinateY += parent.Y;

                // Stop at Page or Window level
                if (parent is Page || parent.Parent == null)
                {
                    break;
                }

                parent = parent.Parent as VisualElement;
            }

            return (screenCoordinateX, screenCoordinateY);
        }

        public static Task<bool> AnimateAsync(
            this VisualElement element, Animation animation, string name, uint rate = 16, uint length = 250, Easing? easing = null)
        {
            element.AbortAnimation(name);

            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate(name, animation, rate, length, easing, (v, c) => taskCompletionSource.TrySetResult(c));

            return taskCompletionSource.Task;
        }

        public static async Task<bool> AnimateColorTransitionAsync(
            this VisualElement self, Color fromColor, Color toColor, Action<Color> callback, uint length = 250, Easing? easing = null)
        {
            await ColorAnimation(self, "ColorTo", t => ColorHelper.Interpolate(fromColor, toColor, t), callback, length, easing);
            return true;
        }

        public static async Task Wobble(this View view)
        {
            await view.RotateTo(5);
            await view.RotateTo(-5);
        }

        public static void CancelAnimation(this VisualElement self)
        {
            self.AbortAnimation("ColorTo");
        }

        private static Task<bool> ColorAnimation(
            VisualElement element, string name, Func<double, Color> transform, Action<Color> callback, uint length, Easing? easing = null)
        {
            element.AbortAnimation(name);

            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.TrySetResult(c));

            return taskCompletionSource.Task;
        }
    }
