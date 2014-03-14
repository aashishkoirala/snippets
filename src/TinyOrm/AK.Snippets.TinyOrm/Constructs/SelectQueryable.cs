/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Constructs.SelectQueryable
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
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.TinyOrm.Constructs
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class SelectQueryable<TProjectedIn, TProjectedOut, TMapped> : SqlQueryable<TProjectedOut, TMapped>
    {
        private readonly SqlQueryable<TProjectedIn, TMapped> target;
        private readonly Expression<Func<TProjectedIn, TProjectedOut>> mapper;

        public SelectQueryable(
            SqlQueryable<TProjectedIn, TMapped> target,
            Expression<Func<TProjectedIn, TProjectedOut>> mapper)
        {
            this.target = target;
            this.mapper = mapper;
        }

        public override Query Parse()
        {
            var query = this.target.Parse();
            if (!query.Select.IsEmpty)
            {
                if (!query.OrderBy.IsEmpty) throw new NotSupportedException();
                query = new Query(query);
            }

            var memberExpression = this.mapper.Body as MemberExpression;
            if (memberExpression != null)
                return HandleMemberExpression(memberExpression, query);

            var newExpression = this.mapper.Body as NewExpression;
            if (newExpression == null) throw new NotSupportedException();

            return HandleNewExpression(newExpression, query);
        }

        private static Query HandleMemberExpression(MemberExpression memberExpression, Query query)
        {
            if (!(memberExpression.Expression is ParameterExpression))
                throw new NotSupportedException();

            if (!query.Select.IsEmpty)
                query.Select.Append(", ");
            query.Select.Append(memberExpression.Member.Name);

            return query;
        }

        private static Query HandleNewExpression(NewExpression newExpression, Query query)
        {
            foreach (var memberExpression in newExpression.Arguments.Select(x => x as MemberExpression))
            {
                if (memberExpression == null) throw new NotSupportedException();
                if (!(memberExpression.Expression is ParameterExpression)) throw new NotSupportedException();

                if (!query.Select.IsEmpty) query.Select.Append(", ");
                query.Select.Append(memberExpression.Member.Name);
            }

            return query;
        }
    }
}
