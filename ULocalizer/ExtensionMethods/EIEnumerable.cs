using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
