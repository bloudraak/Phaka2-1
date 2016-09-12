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
using Phaka.Abstractions;

namespace Phaka
{
    internal class DeploymentResourceProviderAdaptor
    {
        private readonly SetAsyncDelegate _setAsyncDelegate;

        public DeploymentResourceProviderAdaptor(object provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            Type[] types =
            {
                typeof(IDeploymentContext),
                typeof(IDeploymentResource),
                typeof(CancellationToken)
            };
            var method = provider.GetType().GetMethod("SetAsync", types);
            if (method == null)
                throw new InvalidOperationException("The type '" + provider.GetType() +
                                                    "' doesn't have an SetAsync method that takes three parameters.");
            if (typeof(Task) != method.ReturnType)
                throw new InvalidOperationException("The type '" + provider.GetType() +
                                                    "' has a SetAsync method but it doesn't return Task.");
            _setAsyncDelegate = (SetAsyncDelegate) Delegate.CreateDelegate(typeof(SetAsyncDelegate), provider, method);
        }

        public async Task SetAsync(IDeploymentContext context,
            IDeploymentResource resource,
            CancellationToken cancellationToken)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            await _setAsyncDelegate(context, resource, cancellationToken);
        }
    }
}