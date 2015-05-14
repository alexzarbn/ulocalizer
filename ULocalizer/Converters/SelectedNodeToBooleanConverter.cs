using System;
using System.Globalization;
using System.Windows.Data;
using ULocalizer.Classes;

namespace ULocalizer.Converters
{
    /// <summary>
    ///     Converts selected node to boolean
    /// </summary>
    public class SelectedNodeToBooleanConverter : IValueConverter
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
            if ((value != null) && (((CTranslationNode) value).Items != null) && (((CTranslationNode) value).Items.Count > 0))
            {
                return true;
            }
            return false;
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