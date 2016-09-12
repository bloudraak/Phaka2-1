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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Phaka.Abstractions;
using Phaka.Graphs;

namespace Phaka
{
    public class DeploymentManager : IDeploymentManager
    {
        public async Task DeployAsync(IDeploymentContext context,
            IEnumerable<IDeploymentResource> resources,
            CancellationToken cancellationToken)
        {
            //
            // Create a map between resources and activities, so we can easily
            // map dependencies later on
            //
            var order = 0;
            var map = new Dictionary<IDeploymentResource, IDeploymentActivity>();
            foreach (var resource in resources)
            {
                order++;
                var t = typeof(IDeploymentResourceProvider<>);
                var x = t.MakeGenericType(resource.GetType());
                var provider = context.DeploymentServices.GetService(x);
                var adaptor = new DeploymentResourceProviderAdaptor(provider);
                Expression<Func<Task>> expression = () => adaptor.SetAsync(context, resource, cancellationToken);
                var activity = new DeploymentActivity(resource.Key, expression, order);
                map.Add(resource, activity);
            }

            //
            // Add all the acitivies to the graph
            //
            var activityGraph = new Graph<IDeploymentActivity>();
            foreach (var value in map.Values)
                activityGraph.Add(value);

            //
            // Create dependencies between activities
            //
            foreach (var pair in map)
            {
                var resource = pair.Key;
                var activity = pair.Value;

                foreach (var antecedent in resource.Antecedents)
                {
                    var antecedentActivity = map[antecedent];
                    activityGraph.SetAntecedent(activity, antecedentActivity);
                }
            }

            //
            // Execute the activities
            //
            await activityGraph.Walk(activity => activity.ExecuteAsync(), context.Parallel);
        }
    }
}