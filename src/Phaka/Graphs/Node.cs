// The MIT License (MIT)
// 
// Copyright (c) 2016 Werner Strydom
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;

namespace Phaka.Graphs
{
    public class Node<T> : IEquatable<Node<T>>, IEquatable<T>
    {
        private readonly ISet<Node<T>> _antecedents = new HashSet<Node<T>>();
        private readonly ISet<Node<T>> _descendants = new HashSet<Node<T>>();

        internal Node(T value, Graph<T> graph)
        {
            Value = value;
            Graph = graph;
        }

        public T Value { get; }


        public Graph<T> Graph { get; }

        public IEnumerable<Node<T>> Antecedents => _antecedents;

        public IEnumerable<Node<T>> Descendants => _descendants;

        public bool Equals(Node<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() == GetType())
                return Equals((Node<T>) obj);
            if (obj.GetType() == typeof(T))
                return Equals((T) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public static bool operator ==(Node<T> left, Node<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node<T> left, Node<T> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(T left, Node<T> right)
        {
            // flip the order so the equals of this class is called.
            return Equals(right, left);
        }

        public static bool operator !=(T left, Node<T> right)
        {
            // flip the order so the equals of this class is called.
            return !Equals(right, left);
        }

        public static bool operator ==(Node<T> left, T right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node<T> left, T right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        internal void SetAntecedent(Node<T> antecedent)
        {
            if (antecedent == null)
                throw new ArgumentNullException(nameof(antecedent));

            _antecedents.Add(antecedent);
            antecedent._descendants.Add(this);
        }
    }
}