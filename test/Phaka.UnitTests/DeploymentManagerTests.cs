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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Phaka.Abstractions;
using Phaka.Mocks;

namespace Phaka
{
    [TestFixture]
    public class DeploymentManagerTests
    {
        [Test]
        [Repeat(10)]
        public async Task DeployAsync([Values(true, false)] bool parallel)
        {
            // Arrange
            TestContext.WriteLine();
            var resourceA = new MockResource("A") {Delay = TimeSpan.FromMilliseconds(10)};
            var resourceB = new MockResource("B") {Delay = TimeSpan.FromMilliseconds(10)};
            var resourceC = new MockResource("C") {Delay = TimeSpan.FromMilliseconds(10)};
            var resourceD = new MockResource("D") {Delay = TimeSpan.FromMilliseconds(70)};
            var resourceE = new MockResource("E") {Delay = TimeSpan.FromMilliseconds(10)};

            resourceE.AddAntecedent(resourceC);
            resourceC.AddAntecedent(resourceB);
            resourceB.AddAntecedent(resourceA);
            resourceD.AddAntecedent(resourceA);

            var resources = new List<IDeploymentResource>
            {
                resourceA,
                resourceB,
                resourceC,
                resourceD,
                resourceE
            };
            var resourceProvider = new MockDeploymentResourceProvider();
            resourceProvider.Reset();
            IDeploymentContext context = new MockDeploymentContext(resourceProvider) {Parallel = parallel};
            IDeploymentManager target = new DeploymentManager();

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await target.DeployAsync(context, resources, CancellationToken.None);
            stopwatch.Stop();

            // Assert
            TestContext.WriteLine("Elapsed Time: " + stopwatch.Elapsed);
            if (parallel)
            {
                Assert.AreEqual(1, resourceA.CompletedIndex);
                Assert.AreEqual(2, resourceB.CompletedIndex);
                Assert.AreEqual(3, resourceC.CompletedIndex);
                Assert.AreEqual(5, resourceD.CompletedIndex);
                Assert.AreEqual(4, resourceE.CompletedIndex);
            }
            else
            {
                Assert.AreEqual(3, resourceA.CompletedIndex);
                Assert.AreEqual(4, resourceB.CompletedIndex);
                Assert.AreEqual(5, resourceC.CompletedIndex);
                Assert.AreEqual(2, resourceD.CompletedIndex);
                Assert.AreEqual(6, resourceE.CompletedIndex);
            }
        }

        [Test]
        public async Task DeployAsync_InOrder()
        {
            // Arrange
            var resourceA = new MockResource("A") {Delay = TimeSpan.FromMilliseconds(40)};
            var resourceB = new MockResource("B") {Delay = TimeSpan.FromMilliseconds(60)};
            var resourceC = new MockResource("C") {Delay = TimeSpan.FromMilliseconds(80)};

            resourceA.AddAntecedent(resourceB);
            resourceB.AddAntecedent(resourceC);

            var resources = new List<IDeploymentResource>
            {
                resourceA,
                resourceB,
                resourceC
            };
            var resourceProvider = new MockDeploymentResourceProvider();
            IDeploymentContext context = new MockDeploymentContext(resourceProvider) {Parallel = false};
            IDeploymentManager target = new DeploymentManager();

            // Act
            await target.DeployAsync(context, resources, CancellationToken.None);

            // Assert
            Assert.AreEqual(3, resourceA.CompletedIndex);
            Assert.AreEqual(2, resourceB.CompletedIndex);
            Assert.AreEqual(1, resourceC.CompletedIndex);
        }
    }
}