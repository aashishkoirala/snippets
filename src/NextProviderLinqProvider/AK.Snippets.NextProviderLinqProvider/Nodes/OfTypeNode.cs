/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.Nodes.OfTypeNode
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

using System.Linq;

namespace AK.Snippets.NextProviderLinqProvider.Nodes
{
    /// <summary>
    /// Performs the OfType operation - i.e. filters out from a list of TIn's only those items that are
    /// of type TOut and casts them to TOut's.
    /// </summary>
    /// <typeparam name="TIn">Type of item in incoming list.</typeparam>
    /// <typeparam name="TOut">Type of items to filter in.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class OfTypeNode<TIn, TOut> : NodeBase
    {
        public OfTypeNode(NodeBase target) : base(target) {}

        protected override NodeBase ReduceTerminalItem(NodeBase node)
        {
            var value = node.GetTerminalValue<TIn>();
            return value is TOut ? CreateTerminal((TOut) (object) value) : Null;
        }

        protected override NodeBase ReduceTerminalList(NodeBase node)
        {
            var values = node.GetTerminalValues<TIn>();
            return CreateTerminal(values.OfType<TOut>());
        }

        protected override NodeBase ReduceDirectValue(object value)
        {
            return value is TOut ? CreateTerminal((TOut) value) : Null;
        }
    }
}