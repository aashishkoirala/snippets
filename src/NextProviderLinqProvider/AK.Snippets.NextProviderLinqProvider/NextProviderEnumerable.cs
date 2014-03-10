/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.NextProviderEnumerable
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

#region Namespace Imports

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.NextProviderLinqProvider
{
    /// <summary>
    /// A thin wrapper around NextProviderEnumerator so that you can directly use it to enumerate it
    /// and call IEnumerable-based LINQ methods on it. This is used by ExpressionExecutor to execute methods
    /// such as Any, All, First, etc.
    /// </summary>
    /// <typeparam name="TIn">Type of item in the underlying INextProvider resource.</typeparam>
    /// <typeparam name="TOut">The type parameter for the IEnumerable.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class NextProviderEnumerable<TIn, TOut> : IEnumerable<TOut>
    {
        private readonly INextProvider<TIn> resource;
        private readonly Expression expression;

        public NextProviderEnumerable(INextProvider<TIn> resource, Expression expression)
        {
            this.resource = resource;
            this.expression = expression;
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            return new NextProviderEnumerator<TIn, TOut>(this.resource, this.expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}