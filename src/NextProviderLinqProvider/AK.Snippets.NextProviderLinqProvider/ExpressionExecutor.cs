/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.ExpressionExecutor
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace AK.Snippets.NextProviderLinqProvider
{
    /// <summary>
    /// Handles execution of scalar type LINQ calls such as Any, First, etc.
    /// </summary>
    /// <typeparam name="TIn">Type of item in the underlying INextProvider resource.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class ExpressionExecutor<TIn>
    {
        #region Fields

        private readonly INextProvider<TIn> resource;
        private readonly MethodCallExpression expression;
        private readonly Expression targetExpression;
        private readonly Expression secondExpression;
        private readonly Type targetType;

        #endregion

        #region Public

        public ExpressionExecutor(INextProvider<TIn> resource, MethodCallExpression expression)
        {
            this.resource = resource;
            this.expression = expression;

            if (!this.expression.Arguments.Any())
            {
                throw new NotSupportedException(string.Format(
                    "This variant of the {0} method is not supported.", this.expression.Method.Name));
            }
            this.targetExpression = expression.Arguments[0];

            this.targetType = this.targetExpression.Type;
            if (this.targetType.GetInterfaces().Contains(typeof (IQueryable)))
                this.targetType = this.targetType.GetGenericArguments()[0];

            this.secondExpression = this.expression.Arguments.Count > 1 ? this.expression.Arguments[1] : null;
        }

        public TResult Execute<TResult>()
        {
            var methodMap = new Dictionary<string, Func<TResult>>
            {
                {"First", this.ExecuteFirst<TResult>},
                {"FirstOrDefault", this.ExecuteFirstOrDefault<TResult>},
                {"Single", this.ExecuteSingle<TResult>},
                {"SingleOrDefault", this.ExecuteSingleOrDefault<TResult>},
                {"Last", this.ExecuteLast<TResult>},
                {"LastOrDefault", this.ExecuteLastOrDefault<TResult>},
                {"ElementAt", this.ExecuteElementAt<TResult>},
                {"ElementAtOrDefault", this.ExecuteElementAtOrDefault<TResult>},
                {"Any", this.ExecutePredicatedMethod<TResult>},
                {"All", this.ExecutePredicatedMethod<TResult>},
                {"Count", this.ExecutePredicatedMethod<TResult>},
                {"LongCount", this.ExecutePredicatedMethod<TResult>}
            };

            Func<TResult> method;
            if (methodMap.TryGetValue(this.expression.Method.Name, out method))
                return method();

            throw new NotSupportedException(
                string.Format("The method {0} is not supported.", expression.Method.Name));
        }

        #endregion

        #region Executes

        private TResult ExecuteFirst<TResult>()
        {
            var enumerable = new NextProviderEnumerable<TIn, TResult>(this.resource, this.targetExpression);
            var predicate = this.GetPredicateArgument<TResult>();

            return predicate == null ? enumerable.First() : enumerable.First(predicate);
        }

        private TResult ExecuteFirstOrDefault<TResult>()
        {
            var enumerable = new NextProviderEnumerable<TIn, TResult>(this.resource, this.targetExpression);
            var predicate = this.GetPredicateArgument<TResult>();

            return predicate == null ? enumerable.FirstOrDefault() : enumerable.FirstOrDefault(predicate);
        }

        private TResult ExecuteSingle<TResult>()
        {
            var enumerable = new NextProviderEnumerable<TIn, TResult>(this.resource, this.targetExpression);
            var predicate = this.GetPredicateArgument<TResult>();

            return predicate == null ? enumerable.Single() : enumerable.Single(predicate);
        }

        private TResult ExecuteSingleOrDefault<TResult>()
        {
            var enumerable = new NextProviderEnumerable<TIn, TResult>(this.resource, this.targetExpression);
            var predicate = this.GetPredicateArgument<TResult>();

            return predicate == null ? enumerable.SingleOrDefault() : enumerable.SingleOrDefault(predicate);
        }

        private TResult ExecuteLast<TResult>()
        {
            var enumerable = new NextProviderEnumerable<TIn, TResult>(this.resource, this.targetExpression);
            var predicate = this.GetPredicateArgument<TResult>();

            return predicate == null ? enumerable.Last() : enumerable.Last(predicate);
        }

        private TResult ExecuteLastOrDefault<TResult>()
        {
            var enumerable = new NextProviderEnumerable<TIn, TResult>(this.resource, this.targetExpression);
            var predicate = this.GetPredicateArgument<TResult>();

            return predicate == null ? enumerable.LastOrDefault() : enumerable.LastOrDefault(predicate);
        }

        private TResult ExecuteElementAt<TResult>()
        {
            var enumerable = new NextProviderEnumerable<TIn, TResult>(this.resource, this.targetExpression);

            return enumerable.ElementAt(this.GetIndexArgument());
        }

        private TResult ExecuteElementAtOrDefault<TResult>()
        {
            var enumerable = new NextProviderEnumerable<TIn, TResult>(this.resource, this.targetExpression);

            return enumerable.ElementAtOrDefault(this.GetIndexArgument());
        }

        private TResult ExecutePredicatedMethod<TResult>()
        {
            var enumerableType = typeof (NextProviderEnumerable<,>).MakeGenericType(typeof (TIn), this.targetType);
            var enumerableConstructor = enumerableType.GetConstructors()[0];
            var enumerable = enumerableConstructor.Invoke(new object[] {this.resource, this.targetExpression});

            MethodInfo method;
            object predicate;

            this.GetMethodAndPredicate(out method, out predicate);
            var parameters = predicate == null ? new[] {enumerable} : new[] {enumerable, predicate};

            return (TResult) method.Invoke(null, parameters);
        }

        #endregion

        #region Helpers

        private Func<TResult, bool> GetPredicateArgument<TResult>()
        {
            return this.secondExpression == null
                       ? null
                       : ((Expression<Func<TResult, bool>>) ((UnaryExpression) this.secondExpression).Operand).Compile();
        }

        private int GetIndexArgument()
        {
            if (this.secondExpression == null) throw new InvalidOperationException();

            return (int) ((ConstantExpression) this.secondExpression).Value;
        }

        private void GetMethodAndPredicate(out MethodInfo method, out object predicate)
        {
            Func<MethodInfo, bool> methodCriteriaWithPredicate =
                x =>
                x.Name.StartsWith(this.expression.Method.Name) && x.ContainsGenericParameters &&
                x.GetParameters().Length > 1;

            Func<MethodInfo, bool> methodCriteriaWithoutPredicate =
                x =>
                x.Name.StartsWith(this.expression.Method.Name) && x.ContainsGenericParameters &&
                x.GetParameters().Length == 1;

            method = typeof (Enumerable)
                .GetMethods()
                .Single(this.secondExpression == null
                            ? methodCriteriaWithoutPredicate
                            : methodCriteriaWithPredicate)
                .MakeGenericMethod(this.targetType);

            predicate = null;
            if (this.secondExpression == null) return;

            var funcType = typeof (Func<,>).MakeGenericType(this.targetType, typeof (bool));
            var expressionType = typeof (Expression<>).MakeGenericType(funcType);
            var compileMethod = expressionType.GetMethod("Compile", new Type[0]);
            predicate = compileMethod.Invoke(((UnaryExpression) this.secondExpression).Operand, null);
        }

        #endregion
    }
}