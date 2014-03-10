/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.NextProviderExtensions
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Snippet Collection (AKSC).
 *  
 * AKSC is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKSC is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKSC.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

using System.Linq;

namespace AK.Snippets.NextProviderLinqProvider
{
    /// <summary>
    /// Publicly exposed extension method that can be applied to any INextProvider to return an IQueryable that
    /// implements this LINQ provider.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class NextProviderExtensions
    {
        public static IQueryable<TItem> AsQueryable<TItem>(this INextProvider<TItem> resource)
        {
            return new NextProviderQueryable<TItem, TItem>(resource);
        }
    }
}