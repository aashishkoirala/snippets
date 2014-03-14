/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.SqlEnumerable
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
using System.Data.SqlClient;

#endregion

namespace AK.Snippets.TinyOrm
{
    /// <summary>
    /// IEnumerable for SqlEnumerator.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class SqlEnumerable<TProjected, TMapped> : IEnumerable<TProjected>
    {
        private readonly SqlConnection connection;
        private readonly string sql;

        public SqlEnumerable(SqlConnection connection, string sql)
        {
            this.connection = connection;
            this.sql = sql;
        }

        public IEnumerator<TProjected> GetEnumerator()
        {
            return new SqlEnumerator<TProjected, TMapped>(this.sql, this.connection);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}