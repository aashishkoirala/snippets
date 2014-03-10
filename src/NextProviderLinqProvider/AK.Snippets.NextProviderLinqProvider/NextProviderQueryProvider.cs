/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.NextProviderQueryProvider
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
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.NextProviderLinqProvider
{
    /// <summary>
    /// IQueryProvider that uses an INextProvider as the underlying resource. Uses ExpressionExecutor
    /// for Execute and creates a new NextProviderQueryable for CreateQuery - thus accumulating the
    /// cascading LINQ calls.
    /// </summary>
    /// <typeparam name="TIn">Type of item in the underlying INextProvider resource.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class NextProviderQueryProvider<TIn> : IQueryProvider
    {
        private readonly INextProvider<TIn> resource;

        public NextProviderQueryProvider(INextProvider<TIn> resource)
        {
            this.resource = resource;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new NextProviderQueryable<TIn, TElement>(this.resource)
            {
                Provider = this,
                Expression = expression
            };
        }

        public TResult Execute<TResult>(Expression expression)
        {
            if (!(expression is MethodCallExpression)) throw new NotSupportedException();

            var executor = new ExpressionExecutor<TIn>(this.resource, expression as MethodCallExpression);
            return executor.Execute<TResult>();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return this.CreateQuery<object>(expression);
        }

        public object Execute(Expression expression)
        {
            return this.Execute<object>(expression);
        }
    }
}