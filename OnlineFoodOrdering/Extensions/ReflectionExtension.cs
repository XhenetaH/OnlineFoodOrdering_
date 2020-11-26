using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Extensions
{
    public static class ReflectionExtension
    {
        public static string GetPropertyValue<T>(this T item, string propertryName)
        {
            return item.GetType().GetProperty(propertryName).GetValue(item, null).ToString();
        }
    }
}
