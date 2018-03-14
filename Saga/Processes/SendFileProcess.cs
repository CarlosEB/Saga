using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Interfaces;
using Saga.Processes.Base;
using Saga.Workers.Base;
using Saga.Workers.WorkLog;

namespace Saga.Processes
{
    internal class SendFileProcess : ProcessBase
    {
        public override async Task<WorkLog> DoWorkAsync(IList<WorkLog> workLogs, WorkItemBase workItem = null)
        {
            TokenFileWorkResult workResult = GetWorkLog<TokenFileWorkResult>(workLogs);

            Console.WriteLine($"Sending file: {workResult.UntokenizedFileName}");

            await Task.Delay(TimeSpan.FromSeconds(3));
            if (new Random().Next(1, 100) < 50) throw new Exception("SendFileProcess");

            Guid id = await Task.Run(() => Guid.NewGuid());
            Console.WriteLine("Sent file Id: {0}", id);

            SendFileWorkResult result = new SendFileWorkResult
            {
                LogConnectDirect = "Log Connect Direct",
                WorkId = id,
                Success = true
            };

            return new WorkLog(this, result);
        }

        public override async Task<bool> CompensateAsync(WorkLog item, IRoutingSlip routingSlip)
        {
            return await Task.Run(() =>
            {
                Guid fileId = item.Result.WorkId;
                Guid cancelId = Guid.NewGuid();
                Console.WriteLine("Compensate send file {0} - confirmation Id: {1}.", fileId, cancelId);

                return true;
            });
        }

        public override Uri WorkItemEndpointAddress => new Uri("orchestrator://SendFileProcess/Work");

        public override Uri CompensationEndpointAddress => new Uri("orchestrator://SendFileProcess/Cancel");
    }
}