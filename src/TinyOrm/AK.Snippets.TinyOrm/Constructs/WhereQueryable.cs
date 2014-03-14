/*******************************************************************************************************************************
 * AK.Snippets.TinyOrm.Constructs.WhereQueryable
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
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

#endregion

namespace AK.Snippets.TinyOrm.Constructs
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class WhereQueryable<TProjected, TMapped> : SqlQueryable<TProjected, TMapped>
    {
        private readonly SqlQueryable<TProjected, TMapped> target;
        private readonly Expression<Func<TProjected, bool>> predicate;

        private readonly IDictionary<ExpressionType, string> operationMap = new Dictionary
            <ExpressionType, string>
        {
            {ExpressionType.OrElse, "OR"},
            {ExpressionType.AndAlso, "AND"},
            {ExpressionType.Equal, "="},
            {ExpressionType.NotEqual, "<>"},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.GreaterThanOrEqual, ">="},
            {ExpressionType.LessThanOrEqual, "<="},
        };

        public WhereQueryable(
            SqlQueryable<TProjected, TMapped> target,
            Expression<Func<TProjected, bool>> predicate)
        {
            this.target = target;
            this.predicate = predicate;
        }

        public override Query Parse()
        {
            var binaryExpression = this.predicate.Body as BinaryExpression;
            if (binaryExpression == null) throw new NotSupportedException();

            var builder = new StringBuilder();

            var query = this.target.Parse();
            if (!query.Where.IsEmpty)
            {
                builder.AppendFormat("({0}) AND (", query.Where);
                this.BuildClause(binaryExpression, builder);
                builder.AppendFormat(")");
            }
            else this.BuildClause(binaryExpression, builder);

            query.Where.Clear();
            query.Where.Append(builder);

            return query;
        }

        private void BuildClause(Expression expression, StringBuilder builder)
        {
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                string operation;
                if (!this.operationMap.TryGetValue(binaryExpression.NodeType, out operation))
                    throw new NotSupportedException();

                builder.Append("(");
                this.BuildClause(binaryExpression.Left, builder);
                builder.AppendFormat(" {0} ", operation);
                this.BuildClause(binaryExpression.Right, builder);
                builder.Append(")");
                return;
            }

            ConstantExpression constantExpression;
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null)
            {
                constantExpression = expression as ConstantExpression;
                if (constantExpression == null) throw new NotSupportedException();

                builder.Append(GetValueLiteral(constantExpression.Value));
                return;
            }

            if (memberExpression.Expression is ParameterExpression)
            {
                builder.Append(memberExpression.Member.Name);
                return;
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            var fieldInfo = memberExpression.Member as FieldInfo;

            if (propertyInfo == null && fieldInfo == null)
            {
                builder.Append(GetValueLiteral(null));
                return;
            }

            constantExpression = memberExpression.Expression as ConstantExpression;
            if (memberExpression.Expression != null && constantExpression == null)
                throw new NotSupportedException();
            constantExpression = constantExpression ?? Expression.Constant(null);

            var value = propertyInfo != null
                            ? (memberExpression.Expression == null
                                   ? propertyInfo.GetValue(null)
                                   : propertyInfo.GetValue(constantExpression.Value, null))
                            : fieldInfo.GetValue(memberExpression.Expression == null ? null : constantExpression.Value);

            builder.Append(GetValueLiteral(value));
        }

        private static object GetValueLiteral(object value)
        {
            if (value == null) return "NULL";
            if (value is DateTime) return string.Format("'{0}'", value);
            if (value is string) return string.Format("'{0}'", value.ToString().Replace("'", "''"));
            return value;
        }
    }
}
