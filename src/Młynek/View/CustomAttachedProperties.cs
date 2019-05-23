using System.Windows;

namespace Młynek.View
{
    class CustomAttachedProperties
    {
        public static readonly DependencyProperty CoordinatesProperty = DependencyProperty.Register("FieldState", typeof(string), typeof(CustomAttachedProperties));
        public static void SetFieldState(UIElement element, string value)
        {
            element.SetValue(CoordinatesProperty, value);
        }

        public static string GetFieldState(UIElement element)
        {
            return (string)element.GetValue(CoordinatesProperty);
        }
    }
}