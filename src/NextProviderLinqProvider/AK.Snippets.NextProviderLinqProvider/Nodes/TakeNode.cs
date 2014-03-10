/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.Nodes.TakeNode
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
    /// Peforms the Take operation, i.e. takes the first N items.
    /// </summary>
    /// <typeparam name="TIn">Type of item in incoming collection.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class TakeNode<TIn> : NodeBase
    {
        private readonly int count;
        private int itemsTaken;

        public TakeNode(NodeBase target, int count) : base(target)
        {
            this.count = count;
        }

        protected override NodeBase ReduceTerminalItem(NodeBase node)
        {
            var value = node.GetTerminalValue<TIn>();

            if (this.itemsTaken >= this.count) return Null;

            this.itemsTaken++;
            return CreateTerminal(value);
        }

        protected override NodeBase ReduceTerminalList(NodeBase node)
        {
            var values = node.GetTerminalValues<TIn>();
            var itemsToTake = this.count - this.itemsTaken;

            var returnValue = CreateTerminal(itemsToTake == 0 ? values : values.Take(itemsToTake));

            this.itemsTaken += itemsToTake;

            return returnValue;
        }

        protected override NodeBase ReduceDirectValue(object value)
        {
            if (this.itemsTaken >= this.count) return Null;

            this.itemsTaken++;
            return CreateTerminal((TIn) value);
        }
    }
}