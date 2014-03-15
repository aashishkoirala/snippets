/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Constructs.TableQueryable
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
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.TinyOrm.Constructs
{
    /// <summary>
    /// Represents a table, i.e. the leaf level of a query model.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class TableQueryable<TMapped> : SqlQueryable<TMapped, TMapped>
    {
        private readonly string tableName;

        public TableQueryable(string tableName)
        {
            this.tableName = tableName;
            this.Expression = Expression.Constant(this);
        }

        public override Query Parse()
        {
            return new Query(this.tableName);
        }
    }
}
