using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ULocalizer.Classes;
using ULocalizer.Binding;

namespace ULocalizer.Converters
{
    /// <summary>
    /// Converts selected translation to boolean
    /// </summary>
    public class SelectedTranslationToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value != null) && (((CTranslation)value).Nodes != null) && (((CTranslation)value).Nodes.Count > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Not supported at all
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
