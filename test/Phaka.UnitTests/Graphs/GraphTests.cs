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
using System.IO;
using NUnit.Framework;

namespace Phaka.Graphs
{
    [TestFixture]
    public class GraphTests
    {
        [TestCase(DotKind.Dependency)]
        [TestCase(DotKind.Flow)]
        public void Save_Graph1(DotKind kind)
        {
            var target = new Graph<int>();
            target.Add(1);
            target.Add(2);
            target.Add(3);

            target.SetAntecedent(1, 2);
            target.SetAntecedent(2, 3);
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory,
                TestContext.CurrentContext.Test.Name + ".dot");
            TestContext.WriteLine("Path: " + path);

            // Act
            target.Save(path, kind);

            // Assert
        }

        [TestCase(DotKind.Dependency)]
        [TestCase(DotKind.Flow)]
        public void Save_Graph2(DotKind kind)
        {
            var target = new Graph<int>();
            target.Add(1);
            target.Add(2);
            target.Add(3);

            target.SetAntecedent(1, 2);
            target.SetAntecedent(1, 3);
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory,
                TestContext.CurrentContext.Test.Name + ".dot");
            TestContext.WriteLine("Path: " + path);

            // Act
            target.Save(path, kind);

            // Assert
        }

        [Test]
        public void Add_Should_Create_Node_With_Value()
        {
            // Arrange
            var target = new Graph<int>();

            // Act
            var actual = target.Add(5);

            // Assert
            var collection = new List<Node<int>>(target.Nodes);
            Assert.Contains(actual, collection);
            Assert.AreEqual(5, actual.Value);
        }

        [Test]
        [TestCase(DotKind.Dependency)]
        [TestCase(DotKind.Flow)]
        public void Save_Graph3(DotKind kind)
        {
            var target = new Graph<int>();
            target.Add(1);
            target.Add(2);
            target.Add(3);
            target.Add(4);
            target.Add(5);

            target.SetAntecedent(1, 2);
            target.SetAntecedent(2, 3);
            target.SetAntecedent(3, 5);
            target.SetAntecedent(4, 5);
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory,
                TestContext.CurrentContext.Test.Name + ".dot");
            TestContext.WriteLine("Path: " + path);

            // Act
            target.Save(path, kind);

            // Assert
        }

        [Test]
        public void Sort_Should_Return_Topographically_Sorted_Values_From_Antecedents_To_Descendants()
        {
            // Arrange
            var target = new Graph<int>();
            var n1 = target.Add(1);
            var n2 = target.Add(2);
            var n3 = target.Add(3);

            target.SetAntecedent(1, 2);
            target.SetAntecedent(2, 3);

            // Act

            // Assert
            var list = new List<int>(target.Sort(false));
            Console.WriteLine("Sorted List: {0}", string.Join(", ", list));
            Assert.AreEqual(n3.Value, list[0]);
            Assert.AreEqual(n2.Value, list[1]);
            Assert.AreEqual(n1.Value, list[2]);
        }

        [Test]
        public void Sort_Should_Return_Topographically_Sorted_Values_From_Descendants_To_Antecedents()
        {
            // Arrange
            var target = new Graph<int>();
            var n1 = target.Add(1);
            var n2 = target.Add(2);
            var n3 = target.Add(3);

            target.SetAntecedent(1, 2);
            target.SetAntecedent(2, 3);

            // Act
            var list = new List<int>(target.Sort());

            // Assert
            Console.WriteLine("Sorted List: {0}", string.Join(", ", list));
            Assert.AreEqual(n3.Value, list[2]);
            Assert.AreEqual(n2.Value, list[1]);
            Assert.AreEqual(n1.Value, list[0]);
        }
    }
}