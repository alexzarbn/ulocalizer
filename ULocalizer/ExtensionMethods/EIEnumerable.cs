using System.Collections.Generic;
using ULocalizer.Classes;

namespace ULocalizer.ExtensionMethods
{
    public static class EiEnumerable
    {
        public static CObservableList<T> ToObservableList<T>(this IEnumerable<T> container)
        {
            return new CObservableList<T>(container);
        }
    }
}