/*******************************************************************************************************************************
 * AK.Snippets.NextProviderLinqProvider.Nodes.NodeBase
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

using System.Collections.Generic;

namespace AK.Snippets.NextProviderLinqProvider.Nodes
{
    /// <summary>
    /// Node traversal/reduction logic and common functionality for each type of node.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal abstract class NodeBase
    {
        #region Fields/Properties/Methods

        private static readonly NodeBase nullNode = new NullNode();
        private static readonly NodeBase directValueNode = new DirectValueNode();

        protected NodeBase(NodeBase target)
        {
            this.Target = target;
        }

        public NodeBase Target { get; private set; }

        public static NodeBase Null
        {
            get { return nullNode; }
        }

        public static NodeBase DirectValue
        {
            get { return directValueNode; }
        }

        public bool IsTerminal
        {
            get
            {
                var type = this.GetType();
                if (!type.IsGenericType) return false;
                if (type.GetGenericTypeDefinition() != typeof (TerminalNode<>)) return false;

                var typeArgument = type.GetGenericArguments()[0];
                return !typeArgument.IsGenericType || typeArgument.GetGenericTypeDefinition() != typeof (IEnumerable<>);
            }
        }

        public bool IsTerminalList
        {
            get
            {
                var type = this.GetType();
                if (!type.IsGenericType) return false;
                if (type.GetGenericTypeDefinition() != typeof (TerminalNode<>)) return false;

                var typeArgument = type.GetGenericArguments()[0];
                return typeArgument.IsGenericType && typeArgument.GetGenericTypeDefinition() == typeof (IEnumerable<>);
            }
        }

        public bool IsNull
        {
            get { return this == Null; }
        }

        public bool IsDirectValue
        {
            get { return this == DirectValue; }
        }

        public static NodeBase CreateTerminal<T>(T value)
        {
            return new TerminalNode<T>(value);
        }

        public T GetTerminalValue<T>()
        {
            return ((TerminalNode<T>) this).Value;
        }

        public IEnumerable<T> GetTerminalValues<T>()
        {
            return ((TerminalNode<IEnumerable<T>>) this).Value;
        }

        public NodeBase Reduce(object item)
        {
            var node = this.Target;
            while (true)
            {
                if (node.IsTerminal) return this.ReduceTerminalItem(node);
                if (node.IsTerminalList) return this.ReduceTerminalList(node);
                if (node.IsDirectValue) return this.ReduceDirectValue(item);
                if (node.IsNull) return node;

                node = node.Reduce(item);
            }
        }

        #endregion

        #region Abstracts

        protected abstract NodeBase ReduceTerminalItem(NodeBase node);
        protected abstract NodeBase ReduceTerminalList(NodeBase node);
        protected abstract NodeBase ReduceDirectValue(object value);

        #endregion

        #region TerminalNode

        /// <summary>
        /// 
        /// </summary>
        /// <author>Aashish Koirala</author>
        private class TerminalNode<T> : NodeBase
        {
            public TerminalNode(T value) : base(null)
            {
                this.Value = value;
            }

            public T Value { get; private set; }

            protected override NodeBase ReduceTerminalItem(NodeBase node)
            {
                return Null;
            }

            protected override NodeBase ReduceTerminalList(NodeBase node)
            {
                return Null;
            }

            protected override NodeBase ReduceDirectValue(object value)
            {
                return Null;
            }
        }

        #endregion

        #region NullNode

        /// <summary>
        /// 
        /// </summary>
        /// <author>Aashish Koirala</author>
        private class NullNode : NodeBase
        {
            public NullNode() : base(null) {}

            protected override NodeBase ReduceTerminalItem(NodeBase node)
            {
                return Null;
            }

            protected override NodeBase ReduceTerminalList(NodeBase node)
            {
                return Null;
            }

            protected override NodeBase ReduceDirectValue(object value)
            {
                return Null;
            }
        }

        #endregion

        #region DirectValueNode

        /// <summary>
        /// 
        /// </summary>
        /// <author>Aashish Koirala</author>
        private class DirectValueNode : NodeBase
        {
            public DirectValueNode() : base(null) {}

            protected override NodeBase ReduceTerminalItem(NodeBase node)
            {
                return Null;
            }

            protected override NodeBase ReduceTerminalList(NodeBase node)
            {
                return Null;
            }

            protected override NodeBase ReduceDirectValue(object value)
            {
                return Null;
            }
        }

        #endregion
    }
}