/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Constructs.ItemExecutable
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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.TinyOrm.Constructs
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ItemExecutable<TProjected, TMapped> : ExecutableBase<TProjected, TMapped>
    {
        private readonly SqlQueryable<TProjected, TMapped> queryable;
        private readonly Expression<Func<TProjected, bool>> predicate;

        public ItemExecutable(
            SqlQueryable<TProjected, TMapped> queryable,
            Expression<Func<TProjected, bool>> predicate) : base(queryable, predicate)
        {
            this.queryable = queryable;
            this.predicate = predicate;
        }

        public override bool ReturnsEnumerable
        {
            get { return true; }
        }

        public override TResult Execute<TResult>(SqlConnection connection)
        {
            if (typeof (TResult) != typeof (IEnumerable<TProjected>)) throw new NotSupportedException();

            var targetQueryable = this.queryable;
            if (this.predicate != null)
                targetQueryable = new WhereQueryable<TProjected, TMapped>(this.queryable, this.predicate);

            var query = targetQueryable.Parse();
            query.OrderBy.Clear();

            var sql = string.Format(
                "SELECT TOP 1 * FROM ({0}) {1}_{2}", query, query.Name,
                Guid.NewGuid().ToString().Replace("-", ""));

            connection.SendQueryToProfiler(sql);

            return (TResult) (object) new SqlEnumerable<TProjected, TMapped>(connection, sql);
        }
    }
}
