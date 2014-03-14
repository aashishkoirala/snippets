/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Mapping.ITypeMapping
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
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.TinyOrm.Mapping
{
    /// <summary>
    /// Part of the Fluent mapping interface- see <see cref="Mapper"/>.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal interface ITypeMapping
    {
        Type Type { get; }
        string TableName { get; }
        IDictionary<MemberExpression, string> Members { get; }
    }

    /// <summary>
    /// Part of the Fluent mapping interface- see <see cref="Mapper"/>.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface ITypeMapping<T>
    {
        IMemberMapping<T, TMember> Member<TMember>(
            Expression<Func<T, TMember>> member,
            string columnName = null);
    }
}