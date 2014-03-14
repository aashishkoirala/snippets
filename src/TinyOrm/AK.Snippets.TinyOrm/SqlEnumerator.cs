/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.SqlEnumerator
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

using AK.Snippets.TinyOrm.Mapping;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

#endregion

namespace AK.Snippets.TinyOrm
{
    /// <summary>
    /// Enumerator that wraps a SqlDataReader based on the SQL text and SqlConnection
    /// object that is passed in.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class SqlEnumerator<TProjected, TMapped> : IEnumerator<TProjected>
    {
        private readonly string sql;
        private readonly SqlConnection connection;

        private SqlCommand command;
        private SqlDataReader reader;

        public SqlEnumerator(string sql, SqlConnection connection)
        {
            this.sql = sql;
            this.connection = connection;
        }

        public TProjected Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            if (this.reader != null)
            {
                this.reader.Dispose();
                this.reader = null;
            }

            if (this.command == null) return;

            this.command.Dispose();
            this.command = null;
        }

        public bool MoveNext()
        {
            if (this.reader == null)
            {
                this.command = new SqlCommand(this.sql, this.connection);
                this.reader = this.command.ExecuteReader();
            }

            var isNext = this.reader.Read();
            if (isNext) this.Current = this.reader.GetItem<TProjected, TMapped>();

            return isNext;
        }

        public void Reset() {}
    }
}