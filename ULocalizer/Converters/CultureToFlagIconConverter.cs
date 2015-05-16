using System;
using System.Globalization;
using System.Windows.Data;
using ULocalizer.Classes;

namespace ULocalizer.Converters
{
    /// <summary>
    ///     Converts the specified culture to flag icon's path
    /// </summary>
    public class CultureToFlagIconConverter : IValueConverter
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
            return "/Images/flags/" + ((CCulture) value).ISO + ".png";
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