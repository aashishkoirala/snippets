/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.ArrayNextProvider
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
    /// Example implementation of INextProvider that wraps an array.
    /// </summary>
    /// <typeparam name="T">Type of items in the array.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class ArrayNextProvider<T> : INextProvider<T>
    {
        private readonly T[] array;
        private int index = -1;

        public ArrayNextProvider(T[] array)
        {
            this.array = array;
        }

        public bool GetNext(out T item)
        {
            item = default(T);

            this.index++;
            if (this.index == this.array.Length) return false;

            item = this.array[this.index];
            return true;
        }
    }
}