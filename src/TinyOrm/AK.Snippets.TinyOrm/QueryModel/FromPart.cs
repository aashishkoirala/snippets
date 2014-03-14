/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.QueryModel.FromPart
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

namespace AK.Snippets.TinyOrm.QueryModel
{
    /// <summary>
    /// Represents the FROM part of a query.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class FromPart
    {
        private readonly string tableName;
        private readonly Query query;

        public FromPart(string tableName)
        {
            this.tableName = tableName;
        }

        public FromPart(Query query)
        {
            this.query = query;
        }

        public override string ToString()
        {
            return this.tableName ?? string.Format("( {0} ) {1}", this.query, this.query.Name);
        }
    }
}