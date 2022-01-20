using System;
using System.Windows;

namespace DGP.Genshin.DataViewer.Views
{
    public static class FrameworkElementExtensions
    {
        /// <summary> 
        ///   Executes the specified action if the element is loaded or at the loaded event if it's not loaded.
        /// </summary>
        /// <param name="element">The element where the action should be run.</param>
        /// <param name="invokeAction">An action that takes no parameters.</param>
        public static void ExecuteWhenLoaded(this FrameworkElement element, Action invokeAction)
        {
            if (element.IsLoaded)
            {
                element.Dispatcher.Invoke(invokeAction);
            }
            else
            {
                void ElementLoaded(object o, RoutedEventArgs a)
                {
                    element.Loaded -= ElementLoaded;
                    element.Dispatcher.Invoke(invokeAction);
                }

                element.Loaded += ElementLoaded;
            }
        }

        public static bool GoToElementState(this FrameworkElement element, string stateName, bool useTransions)
        {
            return VisualStateManager.GoToElementState(element, stateName, useTransions);
        }
    }
}
