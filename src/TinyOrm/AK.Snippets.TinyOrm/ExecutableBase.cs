/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.ExecutableBase
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

namespace AK.Snippets.TinyOrm
{
    /// <summary>
    /// Base class for executable constructs.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal abstract class ExecutableBase
    {
        public abstract TResult Execute<TResult>(SqlConnection connection);
        public abstract bool ReturnsEnumerable { get; }
    }

    /// <summary>
    /// Base class for executable constructs.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal abstract class ExecutableBase<TProjected, TMapped> : ExecutableBase
    {
        protected readonly SqlQueryable<TProjected, TMapped> Queryable;
        protected readonly Expression<Func<TProjected, bool>> Predicate;

        protected ExecutableBase(
            SqlQueryable<TProjected, TMapped> queryable,
            Expression<Func<TProjected, bool>> predicate)
        {
            this.Queryable = queryable;
            this.Predicate = predicate;
        }
    }
}