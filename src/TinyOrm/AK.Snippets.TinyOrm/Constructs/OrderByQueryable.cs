/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Constructs.OrderByQueryable
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

using AK.Snippets.TinyOrm.QueryModel;
using System;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.TinyOrm.Constructs
{
    /// <summary>
    /// Generates SQL corresponding to OrderBy, OrderByDescending, ThenBy, ThenByDescending calls.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class OrderByQueryable<TProjected, TOrderByMember, TMapped> : SqlQueryable<TProjected, TMapped>
    {
        private readonly SqlQueryable<TProjected, TMapped> target;
        private readonly Expression<Func<TProjected, TOrderByMember>> orderer;
        private readonly bool isDescending;
        private readonly bool isThenBy;

        public OrderByQueryable(
            SqlQueryable<TProjected, TMapped> target,
            Expression<Func<TProjected, TOrderByMember>> orderer,
            bool isDescending,
            bool isThenBy)
        {
            this.target = target;
            this.orderer = orderer;
            this.isDescending = isDescending;
            this.isThenBy = isThenBy;
        }

        public override Query Parse()
        {
            var query = this.target.Parse();

            if (!query.OrderBy.IsEmpty)
            {
                if (!this.isThenBy) throw new NotSupportedException();
                query.OrderBy.Append(", ");
            }

            var memberExpression = this.orderer.Body as MemberExpression;
            if (memberExpression == null) throw new NotSupportedException();

            if (!(memberExpression.Expression is ParameterExpression))
                throw new NotSupportedException();

            query.OrderBy.Append(memberExpression.Member.Name);
            if (this.isDescending) query.OrderBy.Append(" DESC");

            return query;
        }
    }
}
