//-----------------------------------------------------------------------------------
// <copyright file="RemoveItemListExtension.cs" company="RechercherUnProduit.Fr">
//  Copyright statement All right reserved
// </copyright>
//
// --------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Core.Extensions
{
    public static class RemoveItemListExtension
    {
        public static void Remove<T>(this IList<T> items, Func<T, bool> filterFunc)
        {
            var item = items.FirstOrDefault(filterFunc);

            if (item != null) items.Remove(item);
        }
    }
}