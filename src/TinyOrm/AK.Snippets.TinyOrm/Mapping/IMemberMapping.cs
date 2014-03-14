/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Mapping.IMemberMapping
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
using System.Linq.Expressions;

#endregion

// ReSharper disable UnusedTypeParameter

namespace AK.Snippets.TinyOrm.Mapping
{
    /// <summary>
    /// Part of the Fluent mapping interface- see <see cref="Mapper"/>.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IMemberMapping<T, TMember>
    {
        IMemberMapping<T, TNextMember> Member<TNextMember>(
            Expression<Func<T, TNextMember>> member,
            string columnName = null);
    }
}

// ReSharper restore UnusedTypeParameter