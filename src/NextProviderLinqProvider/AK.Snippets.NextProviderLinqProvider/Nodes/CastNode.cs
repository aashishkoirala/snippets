/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.Nodes.CastNode
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
    /// Performs a Cast operation.
    /// </summary>
    /// <typeparam name="TIn">Type of item in incoming list.</typeparam>
    /// <typeparam name="TOut">Type to cast each item to.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class CastNode<TIn, TOut> : NodeBase
    {
        public CastNode(NodeBase target) : base(target) {}

        protected override NodeBase ReduceTerminalItem(NodeBase node)
        {
            var value = node.GetTerminalValue<TIn>();
            if (!(value is TOut)) throw new InvalidCastException();
            return CreateTerminal((TOut) (object) value);
        }

        protected override NodeBase ReduceTerminalList(NodeBase node)
        {
            var values = node.GetTerminalValues<TIn>();
            return CreateTerminal(values.Cast<TOut>());
        }

        protected override NodeBase ReduceDirectValue(object value)
        {
            if (!(value is TOut)) throw new InvalidCastException();
            return CreateTerminal((TOut) value);
        }
    }
}