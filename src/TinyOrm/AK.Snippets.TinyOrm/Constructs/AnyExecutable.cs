/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Constructs.AnyExecutable
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
using System.Data.SqlClient;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.TinyOrm.Constructs
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class AnyExecutable<TProjected, TMapped> : ExecutableBase<TProjected, TMapped>
    {
        public AnyExecutable(
            SqlQueryable<TProjected, TMapped> queryable,
            Expression<Func<TProjected, bool>> predicate) : base(queryable, predicate) {}

        public override bool ReturnsEnumerable
        {
            get { return false; }
        }

        public override TResult Execute<TResult>(SqlConnection connection)
        {
            if (typeof (TResult) != typeof (bool)) throw new NotSupportedException();

            var targetQueryable = this.Queryable;
            if (this.Predicate != null)
                targetQueryable = new WhereQueryable<TProjected, TMapped>(this.Queryable, this.Predicate);

            var query = targetQueryable.Parse();
            query.OrderBy.Clear();
            query.Select.Clear();
            query.Select.Append("1");

            var sql = string.Format("SELECT 1 WHERE EXISTS ( {0} )", query);
            connection.SendQueryToProfiler(sql);

            object scalar;
            using (var command = new SqlCommand(sql, connection))
            {
                scalar = command.ExecuteScalar();
            }

            return (TResult) (object) (scalar != null);
        }
    }
}
