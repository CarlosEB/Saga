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
    internal class MaskFileProcess : ProcessBase
    {
        public override async Task<WorkLog> DoWorkAsync(IList<WorkLog> workLogs, WorkItemBase workItem = null)
        {
            TokenFileWorkResult workResult = GetWorkLog<TokenFileWorkResult>(workLogs);

            Console.WriteLine($"Masking file: {workResult.UntokenizedFileName}");

            dynamic id = await RetryHelper.RetryAsync<Exception>(async () => await PostWorkItemEndpointAddress<Guid>(), 3, TimeSpan.FromSeconds(3), Logger);
            Console.WriteLine("Masked file Id: {0}", id);

            MaskFileWorkResult result = new MaskFileWorkResult
            {
                MaskedFileName = "GNS.ORG001.T0000001.CS_20180129170710_Masked",
                WorkId = id,
                Success = true
            };

            return new WorkLog(this, result);
        }

        public override async Task<bool> CompensateAsync(WorkLog item, IRoutingSlip routingSlip)
        {
            Guid mapperId = item.Result.WorkId; 
            Guid cancelId = await PostCompensationEndpointAddress<Guid>();
            Console.WriteLine("Compensate Mask {0} - confirmation Id: {1}.", mapperId, cancelId);

            return true;
        }

        public override Uri WorkItemEndpointAddress => new Uri("http://localhost:51246/Mask");

        public override Uri CompensationEndpointAddress => new Uri("http://localhost:51246/Cancel");
    }
}