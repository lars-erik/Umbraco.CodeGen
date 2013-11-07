using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.CodeGen
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<string> NonNullOrWhiteSpace(this IEnumerable<string> strings)
        {
            return strings.Where(s => !String.IsNullOrWhiteSpace(s));
        }

        public static CodeExpression[] AsPrimitiveExpressions<T>(this IEnumerable<T> values)
        {
            return values
                .Select(v => new CodePrimitiveExpression(v))
                .Cast<CodeExpression>()
                .ToArray();
        }
    }
}
