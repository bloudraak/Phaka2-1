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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Phaka.Abstractions;

namespace Phaka
{
    public class DeploymentActivity : IDeploymentActivity, IEquatable<DeploymentActivity>
    {
        public DeploymentActivity(string key, Expression<Func<Task>> expression, int order)
        {
            Order = order;
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Key = key;
            Expression = expression;
        }

        private Expression<Func<Task>> Expression { get; }
        public int Order { get; }

        public string Key { get; }


        public async Task ExecuteAsync()
        {
            var func = Expression.Compile();
            await func();
        }

        public int CompareTo(IDeploymentActivity other)
        {
            var result = Order.CompareTo(other.Order);
            return result;
        }

        public bool Equals(DeploymentActivity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Key, other.Key, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as DeploymentActivity;
            return (other != null) && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(Key);
        }

        public override string ToString()
        {
            return $"{Key}";
        }
    }
}