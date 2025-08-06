using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.MAUI.Ex.Extensions
{
    internal static class VisualTreeHelpers
    {
        internal static T? FindAncestor<T>(this IElement? element) where T : IElement
        {
            while (element != null)
            {
                element = element.Parent;
                if (element is T ancestor)
                {
                    return ancestor;
                }
            }
            return default;
        }
    }
}
