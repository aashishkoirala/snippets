/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.Nodes.WhereNode
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
using System.Linq;

#endregion

namespace AK.Snippets.NextProviderLinqProvider.Nodes
{
    /// <summary>
    /// Performs the Where (i.e. filter) operation.
    /// </summary>
    /// <typeparam name="TIn">Type of item in incoming collection.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class WhereNode<TIn> : NodeBase
    {
        private readonly Func<TIn, bool> predicate;

        public WhereNode(NodeBase target, Func<TIn, bool> predicate) : base(target)
        {
            this.predicate = predicate;
        }

        protected override NodeBase ReduceTerminalItem(NodeBase node)
        {
            var value = node.GetTerminalValue<TIn>();
            return this.predicate(value) ? CreateTerminal(value) : Null;
        }

        protected override NodeBase ReduceTerminalList(NodeBase node)
        {
            var values = node.GetTerminalValues<TIn>();

            return CreateTerminal(values.Where(this.predicate));
        }

        protected override NodeBase ReduceDirectValue(object value)
        {
            return this.predicate((TIn) value) ? CreateTerminal((TIn) value) : Null;
        }
    }
}