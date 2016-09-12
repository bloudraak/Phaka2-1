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

using NUnit.Framework;

namespace Phaka.Graphs
{
    [TestFixture]
    public class NodeTests
    {
        private readonly Graph<int> graph = new Graph<int>();

        [Test]
        public void Constructor_Should_Set_Value_Property()
        {
            // Arrange
            var expected = 5;

            // Act
            var target = new Node<int>(expected, graph);
            var actual = target.Value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(5, 5, ExpectedResult = true)]
        [TestCase(5, 6, ExpectedResult = false)]
        public bool Equality_Operator_Between_Node_And_Value(int targetValue, int otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);

            // Act
            return target == otherValue;
        }

        [Test]
        [TestCase(5, 5, ExpectedResult = true)]
        [TestCase(5, 6, ExpectedResult = false)]
        public bool Equality_Operator_Between_Nodes(int targetValue, int otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);
            var other = new Node<int>(otherValue, graph);

            // Act
            return target == other;
        }

        [Test]
        [TestCase(5, 5, ExpectedResult = true)]
        [TestCase(5, 6, ExpectedResult = false)]
        public bool Equality_Operator_Between_Value_And_Node(int targetValue, int otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);

            // Act
            return otherValue == target;
        }

        [Test(ExpectedResult = true)]
        public bool Equals_Between_Node_And_Itself()
        {
            // Arrange
            var target = new Node<int>(5, graph);

            // Act
            return target.Equals(target);
        }

        [Test(ExpectedResult = true)]
        public bool Equals_Between_Node_And_Itself_As_Object()
        {
            // Arrange
            var target = new Node<int>(5, graph);

            // Act
            return target.Equals((object) target);
        }

        [Test]
        [TestCase(5, 5, ExpectedResult = true)]
        [TestCase(5, 6, ExpectedResult = false)]
        public bool Equals_Between_Node_And_Node(int targetValue, int otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);
            var other = new Node<int>(otherValue, graph);


            // Act
            return target.Equals(other);
        }

        [Test(ExpectedResult = true)]
        public bool Equals_Between_Node_And_Node_As_Object()
        {
            // Arrange
            var target = new Node<int>(5, graph);
            var other = new Node<int>(5, graph);

            // Act
            return target.Equals((object) other);
        }

        [Test(ExpectedResult = false)]
        public bool Equals_Between_Node_And_Null_Node()
        {
            // Arrange
            var target = new Node<int>(5, graph);
            Node<int> other = null;

            // Act
            return target.Equals(other);
        }

        [Test]
        [TestCase(5, 5, ExpectedResult = true)]
        [TestCase(5, 6, ExpectedResult = false)]
        public bool Equals_Between_Node_And_Value(int targetValue, int otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);

            // Act
            return target.Equals(otherValue);
        }


        [Test]
        [TestCase(5, "sample", ExpectedResult = false)]
        [TestCase(5, null, ExpectedResult = false)]
        [TestCase(5, 5, ExpectedResult = true)]
        public bool Equals_Between_Node_And_Value_As_Object(int targetValue, object otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);

            // Act
            return target.Equals(otherValue);
        }

        [Test]
        [TestCase(5, ExpectedResult = 5)]
        [TestCase(6, ExpectedResult = 6)]
        public int GetHashCode_Should_Return_HashCode_Of_Value(int value)
        {
            // Arrange
            var target = new Node<int>(value, graph);

            // Act
            return target.GetHashCode();
        }

        [Test]
        [TestCase(5, 5, ExpectedResult = false)]
        [TestCase(5, 6, ExpectedResult = true)]
        public bool NotEquality_Operator_Between_Node_And_Value(int targetValue, int otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);

            // Act
            return target != otherValue;
        }

        [Test]
        [TestCase(5, 5, ExpectedResult = false)]
        [TestCase(5, 6, ExpectedResult = true)]
        public bool NotEquality_Operator_Between_Nodes(int targetValue, int otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);
            var other = new Node<int>(otherValue, graph);

            // Act
            return target != other;
        }

        [Test]
        [TestCase(5, 5, ExpectedResult = false)]
        [TestCase(5, 6, ExpectedResult = true)]
        public bool NotEquality_Operator_Between_Value_And_Node(int targetValue, int otherValue)
        {
            // Arrange
            var target = new Node<int>(targetValue, graph);

            // Act
            return otherValue != target;
        }

        [Test]
        [TestCase(5, ExpectedResult = "5")]
        [TestCase(6, ExpectedResult = "6")]
        public string ToString_Should_Return_Value_Has_String(int value)
        {
            // Arrange
            var target = new Node<int>(value, graph);

            // Act
            return target.ToString();
        }
    }
}