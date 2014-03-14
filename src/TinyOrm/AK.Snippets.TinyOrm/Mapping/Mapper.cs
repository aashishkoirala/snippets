/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Mapping.Mapper
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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace AK.Snippets.TinyOrm.Mapping
{
    /// <summary>
    /// Lets you map types to tables, and define member-level mappings for each table fluently.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class Mapper
    {
        private static readonly IDictionary<Type, ITypeMapping> map = new Dictionary<Type, ITypeMapping>();

        public static ITypeMapping<T> For<T>(string tableName = null)
        {
            var typeMapping = new TypeMapping<T> {TableName = tableName ?? typeof (T).Name};
            map[typeMapping.Type] = typeMapping;

            return typeMapping;
        }

        internal static IDictionary<Type, ITypeMapping> Map
        {
            get { return map; }
        }

        internal static TProjected GetItem<TProjected, TMapped>(this SqlDataReader reader)
        {
            return typeof (TMapped) != typeof (TProjected)
                       ? GetItemForAnonymousType<TProjected, TMapped>(reader)
                       : GetItemForNormalType<TProjected, TMapped>(reader);
        }

        private static TProjected GetItemForNormalType<TProjected, TMapped>(IDataRecord reader)
        {
            var returnValue = Activator.CreateInstance<TProjected>();

            var typeMapping = Map[typeof (TMapped)];
            var propertyNames = typeMapping.Members.ToDictionary(x => x.Key.Member.Name, x => x.Value);

            var propertiesToSet = typeof (TProjected)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && propertyNames.Keys.Contains(x.Name));

            foreach (var property in propertiesToSet)
            {
                var columnName = propertyNames[property.Name];
                var ordinal = reader.GetOrdinal(columnName);
                var fieldValue = reader.GetValue(ordinal);
                if (fieldValue == DBNull.Value) continue;
                property.SetValue(returnValue, fieldValue, null);
            }

            return returnValue;
        }

        private static TProjected GetItemForAnonymousType<TProjected, TMapped>(IDataRecord reader)
        {
            var constructor = typeof (TProjected).GetConstructors().Single();

            var typeMapping = Map[typeof (TMapped)];
            var propertyNames = typeMapping.Members.ToDictionary(x => x.Key.Member.Name, x => x.Value);

            var parameters = constructor.GetParameters()
                .Select(x => reader.GetOrdinal(propertyNames[x.Name]))
                .Select(reader.GetValue)
                .Select(x => x == DBNull.Value ? null : x)
                .ToArray();

            return (TProjected) constructor.Invoke(parameters);
        }

        #region TypeMapping

        internal class TypeMapping<T> : ITypeMapping<T>, ITypeMapping
        {
            public Type Type
            {
                get { return typeof (T); }
            }

            public string TableName { get; set; }
            public IDictionary<MemberExpression, string> Members { get; set; }

            public IMemberMapping<T, TMember> Member<TMember>(
                Expression<Func<T, TMember>> member,
                string columnName = null)
            {
                this.Members = this.Members ?? new Dictionary<MemberExpression, string>();

                var expression = member.Body as MemberExpression;
                if (expression == null) throw new NotSupportedException();

                columnName = columnName ?? expression.Member.Name;

                this.Members[expression] = columnName;

                return new MemberMapping<T, TMember>
                {
                    Expression = member,
                    ColumnName = columnName,
                    Parent = this
                };
            }
        }

        #endregion

        #region MemberMapping

        internal class MemberMapping<T, TMember> : IMemberMapping<T, TMember>
        {
            public Expression<Func<T, TMember>> Expression { get; set; }
            public string ColumnName { get; set; }
            public ITypeMapping<T> Parent { get; set; }

            public IMemberMapping<T, TNextMember> Member<TNextMember>(
                Expression<Func<T, TNextMember>> member,
                string columnName = null)
            {
                return this.Parent.Member(member, columnName);
            }
        }

        #endregion
    }
}