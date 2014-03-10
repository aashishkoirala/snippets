/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.Nodes.DistinctNode
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace AK.Snippets.NextProviderLinqProvider.Nodes
{
    /// <summary>
    /// Performs a Distinct operation.
    /// </summary>
    /// <typeparam name="TIn">Type of items in incoming list.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class DistinctNode<TIn> : NodeBase
    {
        private readonly IEqualityComparer<TIn> equalityComparer;
        private readonly ICollection<TIn> previousValues = new Collection<TIn>();

        public DistinctNode(NodeBase target) : this(target, null) {}

        public DistinctNode(NodeBase target, IEqualityComparer<TIn> equalityComparer)
            : base(target)
        {
            this.equalityComparer = equalityComparer ?? EqualityComparer<TIn>.Default;
        }

        protected override NodeBase ReduceTerminalItem(NodeBase node)
        {
            var value = node.GetTerminalValue<TIn>();
            if (this.previousValues.Contains(value, this.equalityComparer)) return Null;

            this.previousValues.Add(value);
            return CreateTerminal(value);
        }

        protected override NodeBase ReduceTerminalList(NodeBase node)
        {
            var values = node.GetTerminalValues<TIn>().Except(this.previousValues).ToArray();
            foreach (var value in values) this.previousValues.Add(value);

            return CreateTerminal(values);
        }

        protected override NodeBase ReduceDirectValue(object value)
        {
            if (this.previousValues.Contains((TIn) value, this.equalityComparer)) return Null;

            this.previousValues.Add((TIn) value);
            return CreateTerminal((TIn) value);
        }
    }
}