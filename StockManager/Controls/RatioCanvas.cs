using System.Windows;
using System.Windows.Controls;

namespace StockManager.Controls
{
    class RatioCanvas : Panel
    {
        #region LeftRatioProperty

        public static readonly DependencyProperty LeftRatioProperty
            = DependencyProperty.RegisterAttached(
                "LeftRatio",
                typeof(float), typeof(RatioCanvas),
                new FrameworkPropertyMetadata
                {
                    AffectsArrange = true
                });

        public static void SetLeftRatio(UIElement element, float value)
        {
            element.SetValue(LeftRatioProperty, value);
        }

        public static float GetLeftRatio(UIElement element)
        {
            if (element.GetValue(LeftRatioProperty) is float f)
            {
                return f;
            }

            return 0.0f;
        }

        #endregion

        #region TopRatioProperty

        public static readonly DependencyProperty TopRatioProperty
            = DependencyProperty.RegisterAttached(
                "TopRatio",
                typeof(float), typeof(RatioCanvas),
                new FrameworkPropertyMetadata
                {
                    AffectsArrange = true
                });

        public static void SetTopRatio(UIElement element, float value)
        {
            element.SetValue(TopRatioProperty, value);
        }

        public static float GetTopRatio(UIElement element)
        {
            if (element.GetValue(TopRatioProperty) is float f)
            {
                return f;
            }

            return 0.0f;
        }

        #endregion

        #region WidthRatioProperty

        public static readonly DependencyProperty WidthRatioProperty
            = DependencyProperty.RegisterAttached(
                "WidthRatio",
                typeof(float), typeof(RatioCanvas),
                new FrameworkPropertyMetadata
                {
                    AffectsArrange = true
                });

        public static void SetWidthRatio(UIElement element, float value)
        {
            element.SetValue(WidthRatioProperty, value);
        }

        public static float GetWidthRatio(UIElement element)
        {
            if (element.GetValue(WidthRatioProperty) is float f)
            {
                return f;
            }

            return 100.0f;
        }

        #endregion

        #region HeightRatioProperty

        public static readonly DependencyProperty HeightRatioProperty
            = DependencyProperty.RegisterAttached(
                "HeightRatio",
                typeof(float), typeof(RatioCanvas),
                new FrameworkPropertyMetadata
                {
                    AffectsArrange = true
                });

        public static void SetHeightRatio(UIElement element, float value)
        {
            element.SetValue(HeightRatioProperty, value);
        }

        public static float GetHeightRatio(UIElement element)
        {
            if (element.GetValue(HeightRatioProperty) is float f)
            {
                return f;
            }

            return 100.0f;
        }

        #endregion

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            foreach (UIElement child in InternalChildren)
            {
                var leftRatio = GetLeftRatio(child);
                var topRatio = GetTopRatio(child);
                var widthRatio = GetWidthRatio(child);
                var heightRatio = GetHeightRatio(child);

                child.Arrange(new Rect(
                    leftRatio * arrangeBounds.Width / 100.0,
                    topRatio * arrangeBounds.Height / 100.0,
                    widthRatio * arrangeBounds.Width / 100.0,
                    heightRatio * arrangeBounds.Height / 100.0
                ));
            }

            return arrangeBounds;
        }
    }
}
