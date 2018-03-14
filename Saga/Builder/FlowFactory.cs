using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Interfaces;
using Saga.Processes.Base;
using Saga.Processes.Host;
using Saga.Workers;
using Saga.Workers.Base;
using Saga.Workers.Interfaces;

namespace Saga.Builder
{
    public class FlowFactory
    {
        public static IBuilder Create()
        {
            return new Builder();
        }
        internal class Builder : IBuilder
        {
            private readonly IRoutingSlip _routingSlip;
            private readonly IList<IHostProcess> _processHosts;

            private readonly IList<WorkItemBase> _workItems = new List<WorkItemBase>();

            public Builder()
            {
                this._routingSlip = new RoutingSlip.RoutingSlip();
                this._processHosts = new List<IHostProcess>();
            }

            public IBuilder Then()
            {
                return this;
            }

            public IBuilder RunProcess<TProcess>(IWorkItemArguments args)  where TProcess : ProcessBase, new()
            {
                _workItems.Add(new WorkItem<TProcess>(args));
                _processHosts.Add(new HostProcess<TProcess>(SendAsync));
                return this;
            }

            public async Task Start(int startInStep = 1, int times = 0, TimeSpan? retryDelay = null, Action fireIfBrokenStep = null)
            {
                if (startInStep < 1 || startInStep > this._workItems.Count)
                    throw new ArgumentOutOfRangeException(nameof(startInStep));

                WorkItemBase work = this._workItems[startInStep - 1];

                this._routingSlip.ProgressUri = work.CreateProcess().WorkItemEndpointAddress;

                int attempts = 0;

                Console.WriteLine("Begin Flow.");

                await Execute(times, retryDelay, fireIfBrokenStep, attempts);

                Console.WriteLine($"Flow executed. Completed status: {this._routingSlip.IsCompleted}");

                if (_routingSlip.IsCompleted == false)
                {
                    Console.WriteLine($"Last Broken Step: {this._routingSlip.BrokenStep}");
                }
            }

            private async Task Execute(int times, TimeSpan? retryDelay, Action fireIfBrokenStep, int attempts)
            {
                do
                {
                    this._routingSlip.SetWorkItems(this._workItems);
                    await SendAsync(_routingSlip.ProgressUri, _routingSlip);

                    if (this._routingSlip.IsAborted)
                    {
                        Console.WriteLine($"Flow Aborted!!! Canceled by process: {_routingSlip.BrokenStep}");
                        break;
                    }

                    if (this._routingSlip.IsCompleted)
                        break;

                    attempts++;
                    if (attempts > times)
                        break;

                    Console.WriteLine("\nPostBack will be fired.\n");

                    fireIfBrokenStep?.Invoke();

                    if (retryDelay == null)
                    {
                        Console.WriteLine($"Broken step detected. Flow will restart in {this._routingSlip.BrokenStep} immediately.");
                    }
                    else
                    {
                        Console.WriteLine($"Broken step detected. Flow will restart in {this._routingSlip.BrokenStep}. Waiting to retry in: {retryDelay.Value}.");
                        await Task.Delay(retryDelay.Value);
                    }
                } while (true);
            }

            private async Task SendAsync(Uri uri, IRoutingSlip routingSlip)
            {
                foreach (IHostProcess process in this._processHosts)
                    if (await process.AcceptMessageAsync(uri, routingSlip)) break;
            }
        }
    }
}