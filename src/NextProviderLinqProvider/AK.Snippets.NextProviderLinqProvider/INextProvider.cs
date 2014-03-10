/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.INextProvider
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

namespace AK.Snippets.NextProviderLinqProvider
{
    /// <summary>
    /// Represents a "next item provider" i.e. something that will keep giving you the next one in a sequence of items
    /// as long as there is one.
    /// </summary>
    /// <typeparam name="TItem">The type of item in the sequence.</typeparam>
    /// <author>Aashish Koirala</author>
    public interface INextProvider<TItem>
    {
        bool GetNext(out TItem item);
    }
}