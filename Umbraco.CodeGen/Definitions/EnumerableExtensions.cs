using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.CodeGen.Definitions
{
    internal static class EnumerableExtensions
    {
        public static bool NullableSequenceEqual<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return (a == null && b == null) ||
                   (a != null && b != null && a.SequenceEqual(b));
        }
    }
}
