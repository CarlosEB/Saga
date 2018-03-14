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
    internal class MapFileProcess : ProcessBase
    {
        public override async Task<WorkLog> DoWorkAsync(IList<WorkLog> workLogs, WorkItemBase workItem = null)
        {
            MaskFileWorkResult workResult = GetWorkLog<MaskFileWorkResult>(workLogs);

            Console.WriteLine($"Maping file: {workResult.MaskedFileName}");
            
            dynamic id = await RetryHelper.RetryAsync<Exception>(async () => await PostWorkItemEndpointAddress<Guid>(), 3, TimeSpan.FromSeconds(3), Logger);
            Console.WriteLine("Mapped file Id: {0}", id);

            MapFileWorkResult result = new MapFileWorkResult
            {
                ClearingFileId = 100,
                WorkId = id,
                Success = true
            };

            return new WorkLog(this, result);            
        }

        public override async Task<bool> CompensateAsync(WorkLog item, IRoutingSlip routingSlip)
        {
            Guid mapperId = item.Result.WorkId; 
            Guid cancelId = await PostCompensationEndpointAddress<Guid>();
            Console.WriteLine("Compensate mapper {0} - confirmation Id: {1}.", mapperId, cancelId);

            return true;
        }

        public override Uri WorkItemEndpointAddress => new Uri("http://localhost:51246/Map");

        public override Uri CompensationEndpointAddress => new Uri("http://localhost:51246/Cancel");
    }
}