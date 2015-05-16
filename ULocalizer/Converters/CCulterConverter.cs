using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using ULocalizer.Binding;

namespace ULocalizer.Converters
{
    /// <summary>
    ///     Converts string to specified CCulture
    /// </summary>
    public class CCulterConverter : TypeConverter
    {
        // Overrides the CanConvertFrom method of TypeConverter.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
        }

        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = value as string;
            if (s != null)
            {
                var v = Common.Cultures.FirstOrDefault(cCulture => cCulture.ISO == s);
                return v;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}