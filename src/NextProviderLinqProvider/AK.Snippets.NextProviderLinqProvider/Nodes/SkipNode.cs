/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.Nodes.SkipNode
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
    /// Performs the Skip operation, i.e. skips the first N items.
    /// </summary>
    /// <typeparam name="TIn">Type of item in incoming collection.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class SkipNode<TIn> : NodeBase
    {
        private readonly int count;
        private int itemsSkipped;

        public SkipNode(NodeBase target, int count) : base(target)
        {
            this.count = count;
        }

        protected override NodeBase ReduceTerminalItem(NodeBase node)
        {
            var value = node.GetTerminalValue<TIn>();

            if (this.itemsSkipped == this.count)
                return CreateTerminal(value);

            this.itemsSkipped++;
            return Null;
        }

        protected override NodeBase ReduceTerminalList(NodeBase node)
        {
            var values = node.GetTerminalValues<TIn>();
            var itemsToSkip = this.count - this.itemsSkipped;

            var returnValue = CreateTerminal(itemsToSkip == 0 ? values : values.Skip(itemsToSkip));

            this.itemsSkipped += itemsToSkip;
            return returnValue;
        }

        protected override NodeBase ReduceDirectValue(object value)
        {
            if (this.itemsSkipped == this.count)
                return CreateTerminal((TIn) value);

            this.itemsSkipped++;
            return Null;
        }
    }
}