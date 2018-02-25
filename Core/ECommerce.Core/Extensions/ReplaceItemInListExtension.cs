//-----------------------------------------------------------------------------------
// <copyright file="ReplaceItemInListExtension.cs" company="RechercherUnProduit.Fr">
//  Copyright statement All right reserved
// </copyright>
//
// --------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Core.Extensions
{
    public static class ReplaceItemInListExtension
    {
        public static void Replace<T>(this IList<T> items, Func<T, bool> filterFunc, T newValue)
        {
            var oldItem = items.FirstOrDefault(filterFunc);

            if (oldItem == null) return;

            items.Remove(oldItem);
            items.Add(newValue);
        }
    }
}