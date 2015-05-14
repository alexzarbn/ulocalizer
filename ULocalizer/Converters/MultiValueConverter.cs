using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ULocalizer.Converters
{
    /// <summary>
    ///     Converter for multibinding
    /// </summary>
    public class MultiValueConverter : IMultiValueConverter
    {
        /// <summary>
        ///     Converts to single value
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(value => (!(value is bool)) || (bool) value);
        }

        /// <summary>
        ///     Not supported at all
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}