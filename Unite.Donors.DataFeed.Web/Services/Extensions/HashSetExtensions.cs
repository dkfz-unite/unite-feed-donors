using System.Collections.Generic;
using System.Linq;

namespace Unite.Donors.DataFeed.Web.Services.Extensions
{
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> source, IEnumerable<T> values)
        {
            if(values != null && values.Any())
            {
                foreach(var value in values)
                {
                    source.Add(value);
                }
            }
        }
    }
}
