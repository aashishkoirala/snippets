/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.QueryModel.Query
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
using System.Text;

#endregion

namespace AK.Snippets.TinyOrm.QueryModel
{
    /// <summary>
    /// Represents the structure of a SQL query.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class Query
    {
        public Clause Select { get; private set; }
        public FromPart From { get; private set; }
        public Clause Where { get; private set; }
        public Clause OrderBy { get; private set; }
        public string Name { get; private set; }

        private Query()
        {
            this.Select = new Clause();
            this.Where = new Clause();
            this.OrderBy = new Clause();
        }

        public Query(string tableName) : this()
        {
            this.From = new FromPart(tableName);
            this.Name = string.Format("{0}_{1}", tableName, Guid.NewGuid().ToString().Replace("-", ""));
        }

        public Query(Query innerQuery) : this()
        {
            this.From = new FromPart(innerQuery);
            this.Name = string.Format("{0}_{1}", innerQuery.Name, Guid.NewGuid().ToString().Replace("-", ""));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("SELECT ");
            builder.AppendFormat(this.Select.IsEmpty ? "*" : "{0}", this.Select);
            builder.AppendFormat(" FROM {0}", this.From);

            if (!this.Where.IsEmpty)
                builder.AppendFormat(" WHERE {0}", this.Where);

            if (!this.OrderBy.IsEmpty)
                builder.AppendFormat(" ORDER BY {0}", this.OrderBy);

            return builder.ToString();
        }
    }
}