using System;
using System.Threading.Tasks;
using Saga.Interfaces;
using Saga.Processes.Base;

namespace Saga.Processes.Host
{
    public class HostProcess<TProcess> : IHostProcess where TProcess : ProcessBase, new()
    {
        private readonly Func<Uri, IRoutingSlip, Task> _send;

        public HostProcess(Func<Uri, IRoutingSlip, Task> send)
        {
            this._send = send;
        }

        public async Task<bool> AcceptMessageAsync(Uri uri, IRoutingSlip routingSlip)
        {
            TProcess process = new TProcess();

            if (process.WorkItemEndpointAddress.Equals(uri))
            {
                await this.ProcessForwardMessageAsync(routingSlip);
                return true;
            }

            if (!process.CompensationEndpointAddress.Equals(uri)) return false;

            await this.ProcessBackwardMessageAsync(routingSlip);

            return true;
        }

        private async Task ProcessForwardMessageAsync(IRoutingSlip routingSlip)
        {
            if (routingSlip.IsCompleted == false)
            {
                if (await routingSlip.ProcessNextAsync())
                    await this._send(routingSlip.ProgressUri, routingSlip);
                else
                    await this._send(routingSlip.CompensationUri, routingSlip);
            }
        }

        private async Task ProcessBackwardMessageAsync(IRoutingSlip routingSlip)
        {
            if (routingSlip.IsInProgress)
            {
                if (await routingSlip.UndoLastAsync())
                    await this._send(routingSlip.CompensationUri, routingSlip);
                else
                    await this._send(routingSlip.ProgressUri, routingSlip);
            }
        }
    }
}