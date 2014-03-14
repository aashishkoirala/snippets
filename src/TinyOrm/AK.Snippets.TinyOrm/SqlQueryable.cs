/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.SqlQueryable
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
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.TinyOrm
{
    /// <summary>
    /// Base class for all queryable constructs.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal abstract class SqlQueryable<TProjected, TMapped> : IOrderedQueryable<TProjected>
    {
        public SqlConnection Connection { get; set; }
        public IQueryProvider Provider { get; set; }
        public Expression Expression { get; set; }

        public Type ElementType
        {
            get { return typeof (TProjected); }
        }

        public IEnumerator<TProjected> GetEnumerator()
        {
            var sql = this.Parse().ToString();
            this.Connection.SendQueryToProfiler(sql);

            return new SqlEnumerator<TProjected, TMapped>(sql, this.Connection);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public abstract Query Parse();
    }
}