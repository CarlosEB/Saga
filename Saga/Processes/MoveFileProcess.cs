using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Interfaces;
using Saga.Processes.Base;
using Saga.Workers.Base;
using Saga.Workers.WorkLog;

namespace Saga.Processes
{
    internal class MoveFileProcess : ProcessBase
    {
        public override async Task<WorkLog> DoWorkAsync(IList<WorkLog> workLogs, WorkItemBase workItem = null)
        {
            CreateFileWorkResult workResultCreate = GetWorkLog<CreateFileWorkResult>(workLogs);
            TokenFileWorkResult workResultToken = GetWorkLog<TokenFileWorkResult>(workLogs);
            MaskFileWorkResult workResultMask = GetWorkLog<MaskFileWorkResult>(workLogs);

            Console.WriteLine("Moving files:");

            Console.WriteLine(workResultCreate.FileName);
            Console.WriteLine(workResultToken.UntokenizedFileName);
            Console.WriteLine(workResultMask.MaskedFileName);

            await Task.Delay(TimeSpan.FromSeconds(3));
            if (new Random().Next(1, 100) < 50) throw new Exception("MoveFileProcess");

            Guid id = Guid.NewGuid();
            Console.WriteLine("Move file Id: {0}", id);

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
            await Task.Delay(TimeSpan.FromSeconds(3));

            Guid fileId = item.Result.WorkId;
            Guid cancelId = Guid.NewGuid();
            Console.WriteLine("Compensate move file {0} - confirmation Id: {1}.", fileId, cancelId);

            return true;
        }

        public override Uri WorkItemEndpointAddress => new Uri("orchestrator://MoveFileProcess/Work");

        public override Uri CompensationEndpointAddress => new Uri("orchestrator://MoveFileProcess/Cancel");
    }
}