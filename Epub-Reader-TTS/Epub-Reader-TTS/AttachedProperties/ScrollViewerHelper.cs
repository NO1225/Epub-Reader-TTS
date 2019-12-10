using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Scroll an items control to the bottom when the data context changes
    /// </summary>
    public class HorizantalWheelScrollingProperty : BaseAttachedProperty<HorizantalWheelScrollingProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as ScrollViewer; // UIElement;

            if (element == null)
                throw new Exception("Attached property must be used with UIElement.");

            //element.ScrollChanged += OnScrollChanged;

            if ((bool)e.NewValue)
                element.PreviewMouseWheel += OnPreviewMouseWheel;
            else
                element.PreviewMouseWheel -= OnPreviewMouseWheel;
        }

        double firstPosition;
        bool scrollStarted = true;

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            if (scrollStarted)
            {
                firstPosition = 0;
                scrollStarted = false;
            }

            firstPosition += e.HorizontalOffset;

            if (firstPosition >=e.ViewportWidth/2)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + e.ViewportWidth / 2);
                scrollStarted = true;
            }
            else if(firstPosition <= -e.ViewportWidth / 2)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.ViewportWidth / 2);
                scrollStarted = true;
            }

            e.Handled = true;
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            var scrollViewer = sender as ScrollViewer;

            if(scrollViewer==null)
                scrollViewer = ((UIElement)sender).FindDescendant<ScrollViewer>();

            if (scrollViewer == null)
                return;

            if (args.Delta < 0)
                scrollViewer.PageRight();
            else
                scrollViewer.PageLeft();

            args.Handled = true;
        }

    } 
    
}
