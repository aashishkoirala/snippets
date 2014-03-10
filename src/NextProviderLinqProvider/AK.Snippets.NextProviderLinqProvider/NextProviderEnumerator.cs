/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.NextProviderEnumerator
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace AK.Snippets.NextProviderLinqProvider
{
    /// <summary>
    /// Enumerator that is called on when the queryable is enumerated. The MoveNext for this traverses nodes that are parsed
    /// based on the expression that is used to create this enumerator.
    /// </summary>
    /// <typeparam name="TIn">Type of item in the underlying INextProvider resource.</typeparam>
    /// <typeparam name="TOut">The type parameter for the IEnumerator.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class NextProviderEnumerator<TIn, TOut> : IEnumerator<TOut>
    {
        #region Fields

        private readonly INextProvider<TIn> resource;
        private readonly NodeBase rootNode;
        private IEnumerator<TOut> previousResultsEnumerator;

        #endregion

        #region Constructor/IEnumerator Boilerplate

        public NextProviderEnumerator(INextProvider<TIn> resource, Expression expression)
        {
            this.resource = resource;
            this.rootNode = expression.Parse();
        }

        public TOut Current { get; private set; }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public void Dispose() {}
        public void Reset() {}

        #endregion

        #region MoveNext + Evaluate

        public bool MoveNext()
        {
            if (this.previousResultsEnumerator != null)
            {
                if (this.previousResultsEnumerator.MoveNext())
                {
                    this.Current = this.previousResultsEnumerator.Current;
                    return true;
                }
                this.previousResultsEnumerator = null;
            }

            TIn item;
            if (!this.resource.GetNext(out item)) return false;

            TOut result;
            IEnumerable<TOut> results;
            bool isResultList;
            var isThere = Evaluate(item, this.rootNode,
                                   out result, out results, out isResultList);

            if (isThere)
            {
                if (isResultList)
                {
                    this.previousResultsEnumerator = results.GetEnumerator();
                    return this.MoveNext();
                }

                this.Current = result;
                return true;
            }

            return this.MoveNext();
        }

        private static bool Evaluate(
            TIn item,
            NodeBase rootNode,
            out TOut result,
            out IEnumerable<TOut> results,
            out bool isResultList)
        {
            isResultList = false;
            result = default(TOut);
            results = Enumerable.Empty<TOut>();
            var node = rootNode.Reduce(item);

            if (node.IsDirectValue)
            {
                result = (TOut) (object) item;
                return true;
            }

            if (node.IsTerminal)
            {
                result = node.GetTerminalValue<TOut>();
                return true;
            }

            if (node.IsTerminalList)
            {
                results = node.GetTerminalValues<TOut>();
                isResultList = true;
                return true;
            }

            if (node.IsNull) return false;

            throw new NotSupportedException();
        }

        #endregion
    }
}