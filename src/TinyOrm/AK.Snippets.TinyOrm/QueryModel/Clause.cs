/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.QueryModel.Clause
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

using System.Text;

namespace AK.Snippets.TinyOrm.QueryModel
{
    /// <summary>
    /// Represents a query clause.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class Clause
    {
        private readonly StringBuilder builder = new StringBuilder();

        public Clause()
        {
            this.builder = new StringBuilder();
        }

        public bool IsEmpty
        {
            get { return this.builder.Length == 0; }
        }

        public void Append(object value)
        {
            this.builder.Append(value);
        }

        public void Clear()
        {
            this.builder.Clear();
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}