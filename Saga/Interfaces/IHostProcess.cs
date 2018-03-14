using System;
using System.Threading.Tasks;

namespace Saga.Interfaces
{
    internal interface IHostProcess
    {
        Task<bool> AcceptMessageAsync(Uri uri, IRoutingSlip routingSlip);
    }
}