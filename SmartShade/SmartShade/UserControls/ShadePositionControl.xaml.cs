using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartShade.UserControls
{
    public sealed partial class ShadePositionControl : UserControl
    {
        public ShadePositionControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ShadePositionProperty = DependencyProperty.Register(
            "ShadePosition", typeof(double), typeof(ShadePositionControl), new PropertyMetadata(default(double), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var position = e?.NewValue;

            if (position == null)
                return;

            ((ShadePositionControl)d).UpdateShadeHeight((double)position);
        }

        /// <summary>
        /// Using percentage, this property determines how open the shade is
        /// 100 percent is completely open
        /// 0 percent is completely closed
        /// </summary>
        public double ShadePosition
        {
            get { return (double) GetValue(ShadePositionProperty); }
            set { SetValue(ShadePositionProperty, value); }
        }

        private void UpdateShadeHeight(double percentOpen)
        {
            // Set the shade's height by percentage of available height
            ShadeGrid.Height = RootGrid.ActualHeight * (percentOpen * 0.01);
        }
    }
}
