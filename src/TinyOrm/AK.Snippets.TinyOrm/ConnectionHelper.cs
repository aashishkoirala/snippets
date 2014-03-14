/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.ConnectionHelper
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

using AK.Snippets.TinyOrm.Constructs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

#endregion

namespace AK.Snippets.TinyOrm
{
    /// <summary>
    /// Public extension methods to SqlConnection- lets you create a queryable based on the connection,
    /// turn profiling on or off, etc.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class ConnectionHelper
    {
        private static readonly IDictionary<SqlConnection, Action<string>> profileActions =
            new Dictionary<SqlConnection, Action<string>>();

        public static IQueryable<T> Query<T>(this SqlConnection connection)
        {
            return new TableQueryable<T>(typeof (T).Name)
            {
                Connection = connection,
                Provider = new SqlQueryProvider<T>(connection)
            };
        }

        public static void Profile(this SqlConnection connection, Action<string> queryAction)
        {
            profileActions[connection] = queryAction;
        }

        public static void Unprofile(this SqlConnection connection)
        {
            if (!profileActions.ContainsKey(connection)) return;
            profileActions.Remove(connection);
        }

        internal static void SendQueryToProfiler(this SqlConnection connection, string sql)
        {
            if (!profileActions.ContainsKey(connection)) return;

            var profileAction = profileActions[connection];
            if (profileAction == null) return;

            profileAction(sql);
        }
    }
}