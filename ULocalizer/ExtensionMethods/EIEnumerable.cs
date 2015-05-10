using System.Collections.Generic;
using ULocalizer.Classes;

namespace ExtensionMethods
{
    public static class EIEnumerable
    {
        public static CObservableList<T> ToObservableList<T>(this IEnumerable<T> Container)
        {
            return new CObservableList<T>(Container);
        }
    }
}
