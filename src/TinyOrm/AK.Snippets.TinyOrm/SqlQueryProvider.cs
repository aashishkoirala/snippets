/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.SqlQueryProvider
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
using AK.Snippets.TinyOrm.Mapping;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace AK.Snippets.TinyOrm
{
    /// <summary>
    /// Query provider that returns queryable constructs or uses executable constructs to create or execute queries
    /// based on the expressions passed in.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class SqlQueryProvider<TMapped> : IQueryProvider
    {
        private readonly SqlConnection connection;

        public SqlQueryProvider(SqlConnection connection)
        {
            this.connection = connection;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression is ConstantExpression)
            {
                return new TableQueryable<TElement>(Mapper.Map[typeof (TMapped)].TableName)
                {
                    Provider = this,
                    Expression = expression
                };
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null) throw new NotSupportedException();

            return this.CreateQuery<TElement>(methodCallExpression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null) throw new NotSupportedException();

            return Execute<TResult>(methodCallExpression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return this.CreateQuery<object>(expression);
        }

        public object Execute(Expression expression)
        {
            return this.Execute<object>(expression);
        }

        private TResult Execute<TResult>(MethodCallExpression expression)
        {
            var executableTypeMap = new Dictionary<string, Type>
            {
                {"Any", typeof (AnyExecutable<,>)},
                {"Count", typeof (CountExecutable<,>)},
                {"First", typeof (ItemExecutable<,>)},
                {"FirstOrDefault", typeof (ItemExecutable<,>)},
                {"Single", typeof (ItemExecutable<,>)},
                {"SingleOrDefault", typeof (ItemExecutable<,>)}
            };

            var enumerationMap = new Dictionary<string, Func<IEnumerable<TResult>, TResult>>
            {
                {"First", x => x.First()},
                {"FirstOrDefault", x => x.FirstOrDefault()},
                {"Single", x => x.Single()},
                {"SingleOrDefault", x => x.SingleOrDefault()}
            };

            if (expression.Arguments.Count == 0) throw new NotSupportedException();

            var methodName = expression.Method.Name;
            var targetExpression = expression.Arguments[0];

            Type targetType;
            object target;
            this.GetTargetForExecute(targetExpression, out target, out targetType);

            var predicate = expression.Arguments.Count > 1
                                ? ((UnaryExpression) expression.Arguments[1]).Operand
                                : null;

            Type executableOpenType;
            if (!executableTypeMap.TryGetValue(methodName, out executableOpenType))
                throw new NotSupportedException();

            var executableType = executableOpenType.MakeGenericType(targetType, typeof (TMapped));
            var constructor = executableType.GetConstructors().Single();
            var executor = (ExecutableBase) constructor.Invoke(new[] {target, predicate});

            if (!executor.ReturnsEnumerable) return executor.Execute<TResult>(this.connection);

            var enumerable = executor.Execute<IEnumerable<TResult>>(this.connection);
            return enumerationMap[methodName](enumerable);
        }

        private void GetTargetForExecute(Expression targetExpression, out object target, out Type targetType)
        {
            var constantExpression = targetExpression as ConstantExpression;
            if (constantExpression != null)
            {
                target = constantExpression.Value;
                targetType = target.GetType().GetGenericArguments()[0];
                return;
            }

            var methodCallExpression = targetExpression as MethodCallExpression;
            if (methodCallExpression == null) throw new NotSupportedException();

            targetType = methodCallExpression.Method.ReturnType;
            targetType = targetType.GetGenericArguments()[0];
            var method = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Single(x => x.IsGenericMethod && x.Name == "CreateQuery");
            method = method.MakeGenericMethod(targetType);
            target = method.Invoke(this, new object[] {targetExpression});
        }

        private IQueryable<TElement> CreateQuery<TElement>(MethodCallExpression expression)
        {
            if (expression.Arguments.Count < 2) throw new NotSupportedException();

            var methodName = expression.Method.Name;
            var target = expression.Arguments[0];
            var operand = ((UnaryExpression) expression.Arguments[1]).Operand;

            var queryableGetterMap = new Dictionary
                <string, Func<Expression, Expression, SqlQueryable<TElement, TMapped>>>
            {
                {"Where", this.CreateWhereQuery<TElement>},
                {"Select", this.CreateSelectQuery<TElement>},
                {"OrderBy", (x, y) => this.CreateOrderByQuery<TElement>(x, y, false, false)},
                {"OrderByDescending", (x, y) => this.CreateOrderByQuery<TElement>(x, y, true, false)},
                {"ThenBy", (x, y) => this.CreateOrderByQuery<TElement>(x, y, false, true)},
                {"ThenByDescending", (x, y) => this.CreateOrderByQuery<TElement>(x, y, true, true)},
            };

            Func<Expression, Expression, SqlQueryable<TElement, TMapped>> queryableGetter;
            if (!queryableGetterMap.TryGetValue(methodName, out queryableGetter))
                throw new NotSupportedException();

            var queryable = queryableGetter(target, operand);

            queryable.Provider = this;
            queryable.Expression = expression;
            queryable.Connection = this.connection;

            return queryable;
        }

        private SqlQueryable<TElement, TMapped> CreateWhereQuery<TElement>(
            Expression target, Expression operand)
        {
            var targetQueryable = (SqlQueryable<TElement, TMapped>) this.CreateQuery<TElement>(target);
            var predicate = (Expression<Func<TElement, bool>>) operand;

            return new WhereQueryable<TElement, TMapped>(targetQueryable, predicate);
        }

        private SqlQueryable<TElement, TMapped> CreateSelectQuery<TElement>(
            Expression target, Expression operand)
        {
            var inType = target.Type.GetGenericArguments()[0];

            var createQueryMethodOpen = this.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(x => x.IsGenericMethod && x.Name == "CreateQuery");
            var createQueryMethod = createQueryMethodOpen.MakeGenericMethod(inType);

            var targetQueryable = createQueryMethod.Invoke(this, new object[] {target});

            var selectQueryableType = typeof (SelectQueryable<,,>)
                .MakeGenericType(inType, typeof (TElement), typeof (TMapped));

            var constructor = selectQueryableType.GetConstructors().Single();

            return (SqlQueryable<TElement, TMapped>) constructor.Invoke(new[] {targetQueryable, operand});
        }

        private SqlQueryable<TElement, TMapped> CreateOrderByQuery<TElement>(
            Expression target, Expression operand, bool isDescending, bool isThenBy)
        {
            var targetQueryable = (SqlQueryable<TElement, TMapped>) this.CreateQuery<TElement>(target);

            var memberType = operand.GetType()
                .GetGenericArguments()[0]
                .GetGenericArguments()[1];

            var orderByQueryableType = typeof (OrderByQueryable<,,>)
                .MakeGenericType(typeof (TElement), memberType, typeof (TMapped));
            var constructor = orderByQueryableType.GetConstructors().Single();

            return (SqlQueryable<TElement, TMapped>) constructor.Invoke(new object[]
            {
                targetQueryable,
                operand,
                isDescending,
                isThenBy
            });
        }
    }
}