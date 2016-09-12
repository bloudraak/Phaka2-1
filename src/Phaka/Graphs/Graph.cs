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
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phaka.Extensions;

namespace Phaka.Graphs
{
    public class Graph<T>
    {
        private readonly IDictionary<T, Node<T>> _nodes;

        public Graph()
        {
            _nodes = new SortedDictionary<T, Node<T>>();
        }

        public IEnumerable<Node<T>> Nodes => _nodes.Values;

        public Node<T> Add(T value)
        {
            return _nodes.GetOrAdd(value, arg => new Node<T>(arg, this));
        }

        public void SetAntecedent(T value, T antecedent)
        {
            var valueNode = Add(value);
            var antecedentNode = Add(antecedent);
            valueNode.SetAntecedent(antecedentNode);
        }

        public IEnumerable<T> Sort(bool descending = true)
        {
            Stack<Node<T>> stack;
            if (descending)
                stack = SortDescending();
            else
                stack = SortAscending();

            return stack.Select(item => item.Value);
        }

        private Stack<Node<T>> SortDescending()
        {
            var stack = new Stack<Node<T>>();
            var visited = new HashSet<Node<T>>();

            foreach (var node in _nodes.Values)
                if (!visited.Contains(node))
                {
                    visited.Add(node);
                    InternalSortDescending(node, stack, visited);
                }
            return stack;
        }

        private void InternalSortDescending(Node<T> node, Stack<Node<T>> stack, ISet<Node<T>> visited)
        {
            foreach (var item in node.Antecedents)
                if (!visited.Contains(item))
                {
                    visited.Add(item);
                    InternalSortDescending(item, stack, visited);
                }
            stack.Push(node);
        }

        private Stack<Node<T>> SortAscending()
        {
            var stack = new Stack<Node<T>>();
            var visited = new HashSet<Node<T>>();

            foreach (var node in _nodes.Values)
                if (!visited.Contains(node))
                {
                    visited.Add(node);
                    InternalSortAscending(node, stack, visited);
                }
            return stack;
        }

        private void InternalSortAscending(Node<T> node, Stack<Node<T>> stack, ISet<Node<T>> visited)
        {
            foreach (var item in node.Descendants)
                if (!visited.Contains(item))
                {
                    visited.Add(item);
                    InternalSortAscending(item, stack, visited);
                }
            stack.Push(node);
        }

        public async Task Walk(Func<T, Task> func, bool parallel = true)
        {
            IDictionary<Node<T>, Task> map = new ConcurrentDictionary<Node<T>, Task>();
            var nodes = _nodes.Values.Where(n => !n.Descendants.Any()).ToArray();
            await Walk(func, nodes, map, parallel);
        }

        private async Task Walk(Func<T, Task> func,
            IEnumerable<Node<T>> nodes,
            IDictionary<Node<T>, Task> map,
            bool parallel)
        {
            if (parallel)
            {
                var tasks = new List<Task>();
                foreach (var node in nodes)
                {
                    Task task;
                    if (!map.TryGetValue(node, out task))
                    {
                        task = Walk(node, func, map, true);
                        map.Add(node, task);
                    }
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
            }
            else
            {
                foreach (var node in nodes)
                    await Walk(node, func, map, false);
            }
        }

        private async Task Walk(Node<T> node,
            Func<T, Task> func,
            IDictionary<Node<T>, Task> map,
            bool parallel)
        {
            await Walk(func, node.Antecedents, map, parallel);
            await func(node.Value);
        }

        public void Save(string path, DotKind kind)
        {
            Save(path, Encoding.ASCII, kind);
        }

        public void Save(string path, Encoding encoding, DotKind kind)
        {
            using (var stream = File.Create(path))
            {
                Save(stream, encoding, kind);
            }
        }

        public void Save(Stream stream, Encoding encoding, DotKind kind)
        {
            using (var writer = new StreamWriter(stream, encoding))
            {
                Save(writer, kind);
            }
        }

        public void Save(TextWriter textWriter, DotKind kind)
        {
            using (var writer = new IndentedTextWriter(textWriter))
            {
                var ids = new Dictionary<Node<T>, string>();

                writer.WriteLine("digraph graphname {");
                writer.Indent++;
                writer.WriteLine("node [shape=doublecircle]; Start;");
                writer.Write("node [shape=circle];");
                foreach (var node in Nodes)
                {
                    ids.Add(node, node.Value.ToString());
                    writer.Write(" {0}", ids[node]);
                }
                writer.WriteLine(";");

                if (kind == DotKind.Dependency)
                {
                    var startNodes = Nodes.Where(item => !item.Antecedents.Any());
                    foreach (var item in startNodes)
                        writer.WriteLine("End -> {0};", ids[item]);

                    var endNodes = Nodes.Where(item => !item.Descendants.Any());
                    foreach (var item in endNodes)
                        writer.WriteLine("{0} -> Start;", ids[item]);
                }
                else
                {
                    var startNodes = Nodes.Where(item => !item.Antecedents.Any());
                    foreach (var item in startNodes)
                        writer.WriteLine("{0} -> End;", ids[item]);

                    var endNodes = Nodes.Where(item => !item.Descendants.Any());
                    foreach (var item in endNodes)
                        writer.WriteLine("Start -> {0};", ids[item]);
                }


                foreach (var node in Nodes)
                {
                    var enumerable = kind == DotKind.Dependency ? node.Descendants : node.Antecedents;
                    var items = new HashSet<Node<T>>(enumerable);

                    foreach (var item in items)
                        writer.WriteLine("{0} -> {1};", ids[node], ids[item]);
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
        }
    }
}