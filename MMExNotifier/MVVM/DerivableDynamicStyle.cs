using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MMExNotifier.MVVM
{
    internal class DerivableDynamicStyle
    {
        public static Style GetBaseStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(BaseStyleProperty);
        }

        public static void SetBaseStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(BaseStyleProperty, value);
        }

        public static readonly DependencyProperty BaseStyleProperty =
            DependencyProperty.RegisterAttached("BaseStyle", typeof(Style), typeof(FrameworkElement), new UIPropertyMetadata(StyleChanged));

        public static Style GetDerivedStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(DerivedStyleProperty);
        }

        public static void SetDerivedStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DerivedStyleProperty, value);
        }

        public static readonly DependencyProperty DerivedStyleProperty =
            DependencyProperty.RegisterAttached("DerivedStyle", typeof(Style), typeof(FrameworkElement), new UIPropertyMetadata(StyleChanged));


        private static void StyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Style baseStyle = GetBaseStyle(d);
            Style derivedStyle = GetDerivedStyle(d);
            Style newStyle;

            if (derivedStyle == null)
            {
                newStyle = baseStyle;
            }
            else
            {
                newStyle = new Style { BasedOn = baseStyle, TargetType = derivedStyle.TargetType };

                foreach (var setter in derivedStyle.Setters)
                {
                    newStyle.Setters.Add(setter);
                }

                foreach (var trigger in derivedStyle.Triggers)
                {
                    newStyle.Triggers.Add(trigger);
                }
            }

            var fe = (FrameworkElement)d;
            fe.Style = newStyle;
        }
    }
}
