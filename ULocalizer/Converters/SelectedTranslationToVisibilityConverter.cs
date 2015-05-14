using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ULocalizer.Classes;

namespace ULocalizer.Converters
{
    /// <summary>
    ///     Converts selected translation to visibility
    /// </summary>
    public class SelectedTranslationToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value != null) && (((CTranslation) value).Nodes != null) && (((CTranslation) value).Nodes.Count > 0))
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        /// <summary>
        ///     Not supported at all
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}