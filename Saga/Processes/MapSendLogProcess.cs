using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Interfaces;
using Saga.Processes.Base;
using Saga.Util;
using Saga.Workers.Base;
using Saga.Workers.WorkLog;

namespace Saga.Processes
{
    internal class MapSendLogProcess : ProcessBase
    {
        public override async Task<WorkLog> DoWorkAsync(IList<WorkLog> workLogs, WorkItemBase workItem = null)
        {
            SendFileWorkResult workResult = GetWorkLog<SendFileWorkResult>(workLogs);

            Console.WriteLine($"Mapping send log: {workResult.LogConnectDirect}");

            dynamic id = await RetryHelper.RetryAsync<Exception>(async () => await PostWorkItemEndpointAddress<Guid>(), 3, TimeSpan.FromSeconds(3), Logger);
            Console.WriteLine("Mapped send log Id: {0}", id);

            MapSendLogWorkResult result = new MapSendLogWorkResult
            {
                SendLogId = 200,
                WorkId = id,
                Success = true
            };

            return new WorkLog(this, result);
        }

        public override async Task<bool> CompensateAsync(WorkLog item, IRoutingSlip routingSlip)
        {
            Guid mapperId = item.Result.WorkId;
            Guid cancelId = await PostCompensationEndpointAddress<Guid>();
            Console.WriteLine("Compensate Mapped send log {0} - confirmation Id: {1}.", mapperId, cancelId);

            return true;
        }

        public override Uri WorkItemEndpointAddress => new Uri("http://localhost:51246/MapSendLog");

        public override Uri CompensationEndpointAddress => new Uri("http://localhost:51246/Cancel");
    }
}