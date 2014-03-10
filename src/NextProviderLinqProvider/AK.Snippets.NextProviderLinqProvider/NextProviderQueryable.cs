/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.NextProviderQueryable
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.NextProviderLinqProvider
{
    /// <summary>
    /// IQueryable that uses an INextProvider as the underlying resource. All this does is implement IQueryable, and
    /// use NextProviderQueryProvider as the provider and NextProviderEnumerator as the enumerator.
    /// </summary>
    /// <typeparam name="TIn">Type of item in the underlying INextProvider resource.</typeparam>
    /// <typeparam name="TOut">
    /// The type parameter for the IQueryable. When created for the first time, always the same as <typeparamref name="TIn"/>.
    /// </typeparam>
    /// <author>Aashish Koirala</author>
    internal class NextProviderQueryable<TIn, TOut> : IQueryable<TOut>
    {
        private readonly INextProvider<TIn> resource;

        public NextProviderQueryable(INextProvider<TIn> resource)
        {
            this.resource = resource;
            this.Expression = Expression.Constant(this);
            this.Provider = new NextProviderQueryProvider<TIn>(this.resource);
        }

        public IQueryProvider Provider { get; set; }
        public Expression Expression { get; set; }

        public Type ElementType
        {
            get { return typeof (TOut); }
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            return new NextProviderEnumerator<TIn, TOut>(this.resource, this.Expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}