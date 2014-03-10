/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.ExpressionParser
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

using AK.Snippets.NextProviderLinqProvider.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.NextProviderLinqProvider
{
    /// <summary>
    /// Parses a LINQ expression that represents cascaded calls to an IQueryable into a linked series of <see cref="NodeBase"/>
    /// items (and returns the root <see cref="NodeBase"/> item). The root node can then be evaluated upon enumeration. This
    /// class handles LINQ calls that create further queries (such as Where, Select, Cast, etc.). Calls that execute scalar
    /// such as Any, First, etc. are handled by the ExpressionExecutor class.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal static class ExpressionParser
    {
        #region Parse Method Map & Public "Parse"

        private static readonly IDictionary<string, Func<MethodCallExpression, NodeBase>> parseMap =
            new Dictionary<string, Func<MethodCallExpression, NodeBase>>
            {
                {"Where", ParseWhere},
                {"Select", ParseSelect},
                {"SelectMany", ParseSelectMany},
                {"Cast", ParseCast},
                {"OfType", ParseOfType},
                {"Distinct", ParseDistinct},
                {"Skip", ParseSkip},
                {"Take", ParseTake}
            };

        public static NodeBase Parse(this Expression expression)
        {
            if (expression is ConstantExpression) return NodeBase.DirectValue;

            if (!(expression is MethodCallExpression))
                throw new NotSupportedException();

            var methodCallExpression = expression as MethodCallExpression;
            var methodName = methodCallExpression.Method.Name;

            Func<MethodCallExpression, NodeBase> parseMethod;
            if (parseMap.TryGetValue(methodName, out parseMethod))
                return parseMethod(methodCallExpression);

            throw new NotSupportedException(string.Format("The method {0} is not supported.", methodName));
        }

        #endregion

        #region Parses

        private static NodeBase ParseWhere(MethodCallExpression expression)
        {
            NodeBase targetNode;
            Type inType, outType;
            GetTarget(expression, out targetNode, out inType, out outType);

            outType = typeof (bool);
            var predicate = GetUnaryOperand(expression, inType, outType);

            return CreateNode(typeof (WhereNode<>), new[] {inType}, targetNode, predicate);
        }

        private static NodeBase ParseSelect(MethodCallExpression expression)
        {
            NodeBase targetNode;
            Type inType, outType;
            GetTarget(expression, out targetNode, out inType, out outType);

            var selector = GetUnaryOperand(expression, inType, outType);

            return CreateNode(typeof (SelectNode<,>), new[] {inType, outType}, targetNode, selector);
        }

        private static NodeBase ParseSelectMany(MethodCallExpression expression)
        {
            NodeBase targetNode;
            Type inType, outType;
            GetTarget(expression, out targetNode, out inType, out outType);

            outType = expression.Type;
            if (outType.GetInterfaces().Contains(typeof (IQueryable)))
            {
                outType = outType.GetGenericArguments()[0];
                outType = typeof (IEnumerable<>).MakeGenericType(outType);
            }

            var selector = GetUnaryOperand(expression, inType, outType);

            return CreateNode(typeof (SelectNode<,>), new[] {inType, outType}, targetNode, selector);
        }

        private static NodeBase ParseCast(MethodCallExpression expression)
        {
            NodeBase targetNode;
            Type inType, outType;
            GetTarget(expression, out targetNode, out inType, out outType);

            return CreateNode(typeof (CastNode<,>), new[] {inType, outType}, targetNode);
        }

        private static NodeBase ParseOfType(MethodCallExpression expression)
        {
            NodeBase targetNode;
            Type inType, outType;
            GetTarget(expression, out targetNode, out inType, out outType);

            return CreateNode(typeof (OfTypeNode<,>), new[] {inType, outType}, targetNode);
        }

        private static NodeBase ParseDistinct(MethodCallExpression expression)
        {
            NodeBase targetNode;
            Type inType, outType;
            GetTarget(expression, out targetNode, out inType, out outType);

            var equalityComparer =
                expression.Arguments.Count() < 2 || !(expression.Arguments[1] is ConstantExpression)
                    ? null
                    : (expression.Arguments[1] as ConstantExpression).Value;

            return equalityComparer == null
                       ? CreateNode(typeof (DistinctNode<>), new[] {inType}, targetNode)
                       : CreateNode(typeof (DistinctNode<>), new[] {inType}, targetNode, equalityComparer);
        }

        private static NodeBase ParseSkip(MethodCallExpression expression)
        {
            NodeBase targetNode;
            Type inType, outType;
            GetTarget(expression, out targetNode, out inType, out outType);

            var count = ((ConstantExpression) expression.Arguments[1]).Value;

            return CreateNode(typeof (SkipNode<>), new[] {inType}, targetNode, count);
        }

        private static NodeBase ParseTake(MethodCallExpression expression)
        {
            NodeBase targetNode;
            Type inType, outType;
            GetTarget(expression, out targetNode, out inType, out outType);

            var count = ((ConstantExpression) expression.Arguments[1]).Value;

            return CreateNode(typeof (TakeNode<>), new[] {inType}, targetNode, count);
        }

        #endregion

        #region Helpers

        private static void GetTarget(MethodCallExpression expression,
                                      out NodeBase targetNode, out Type inType, out Type outType)
        {
            if (!expression.Arguments.Any())
            {
                throw new NotSupportedException(string.Format(
                    "This variant of the {0} method is not supported.", expression.Method.Name));
            }
            var targetExpression = expression.Arguments[0];

            targetNode = targetExpression.Parse();
            inType = targetExpression.Type;
            if (inType.GetInterfaces().Contains(typeof (IQueryable)))
                inType = inType.GetGenericArguments()[0];

            outType = expression.Type;
            if (outType.GetInterfaces().Contains(typeof (IQueryable)))
                outType = outType.GetGenericArguments()[0];
        }

        private static NodeBase CreateNode(Type nodeType, Type[] typeParameters, NodeBase targetNode,
                                           params object[] arguments)
        {
            var closedType = nodeType.MakeGenericType(typeParameters);
            var constructorArguments = new object[] {targetNode}.Union(arguments).ToArray();
            var constructorArgumentTypes = constructorArguments.Select(x => x.GetType()).ToArray();

            var constructor = closedType.GetConstructor(constructorArgumentTypes);
            if (constructor == null) throw new InvalidOperationException();

            return (NodeBase) constructor.Invoke(constructorArguments);
        }

        private static object GetUnaryOperand(
            MethodCallExpression expression, Type inType, Type outType)
        {
            if (expression.Arguments.Count() < 2 || !(expression.Arguments[1] is UnaryExpression))
            {
                throw new NotSupportedException(string.Format(
                    "This variant of the {0} method is not supported.", expression.Method.Name));
            }

            var unaryExpression = expression.Arguments[1] as UnaryExpression;
            var operandExpression = unaryExpression.Operand;

            var funcType = typeof (Func<,>).MakeGenericType(inType, outType);
            var expressionType = typeof (Expression<>).MakeGenericType(funcType);
            var compileMethod = expressionType.GetMethod("Compile", new Type[0]);
            return compileMethod.Invoke(operandExpression, null);
        }

        #endregion
    }
}