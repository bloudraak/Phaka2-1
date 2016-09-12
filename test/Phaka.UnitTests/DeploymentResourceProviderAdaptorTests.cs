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
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Phaka.Mocks;

namespace Phaka
{
    [TestFixture]
    public class DeploymentResourceProviderAdaptorTests
    {
        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_When_Passed_Null()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => new DeploymentResourceProviderAdaptor(null));
        }

        [Test]
        public void Constructor_Should_Throw_If_Method_Doesnt_Have_Correct_Parameters()
        {
            // Act
            var provider = new StubResourceProvider4();
            Assert.Throws<InvalidOperationException>(() => new DeploymentResourceProviderAdaptor(provider));
        }

        [Test]
        public void Constructor_Should_Throw_If_Method_Isnt_Task()
        {
            // Act
            var provider = new StubResourceProvider2();
            Assert.Throws<InvalidOperationException>(() => new DeploymentResourceProviderAdaptor(provider));
        }

        [Test]
        public void Constructor_Should_Throw_If_There_Is_No_SetAsync()
        {
            // Act
            var provider = new StubResourceProvider3();
            Assert.Throws<InvalidOperationException>(() => new DeploymentResourceProviderAdaptor(provider));
        }

        [Test]
        public async Task Constructor_Should_Wrap_Provider()
        {
            // Act
            var provider = new StubResourceProvider1();
            var target = new DeploymentResourceProviderAdaptor(provider);
            var resource = new MockResource(null);
            var resourceProvider = new MockDeploymentResourceProvider();
            var context = new MockDeploymentContext(resourceProvider);
            await target.SetAsync(context, resource, CancellationToken.None);

            //Assert
            Assert.IsTrue(provider.BeenCalled);
        }
    }
}