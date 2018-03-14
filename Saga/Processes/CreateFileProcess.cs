using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Saga.Interfaces;
using Saga.Processes.Base;
using Saga.Util;
using Saga.Workers.Base;
using Saga.Workers.Interfaces;
using Saga.Workers.WorkLog;

namespace Saga.Processes
{
    internal class CreateFileProcess : ProcessBase
    {
        public override async Task<WorkLog> DoWorkAsync(IList<WorkLog> workLogs, WorkItemBase workItem = null)
        {
            // TODO --> Validar data local com base na UTC e corrigir, caso necessário.

            if (workItem == null)
                throw new ArgumentNullException($"{nameof(workItem)} must be informed to CreateFileProcess.");

            DateTime sendDate = ((ICreateFileWorkItemArguments)workItem.Arguments).SendDate;

            Console.WriteLine($"Creating file using parameter: {sendDate }");

            string toSend = JsonConvert.SerializeObject(new { SendDate = sendDate });

            dynamic id = await RetryHelper.RetryAsync<Exception>(async () => await PostWorkItemEndpointAddress<Guid>(toSend), 3, TimeSpan.FromSeconds(3), Logger);
            Console.WriteLine("Created file Id: {0}", id);

            CreateFileWorkResult result = new CreateFileWorkResult
            {
                FileName = "GNS.ORG001.T0000001.CS_20180129170710",
                WorkId = id,
                Success = true
            };

            return new WorkLog(this, result);
        }

        public override async Task<bool> CompensateAsync(WorkLog item, IRoutingSlip routingSlip)
        {
            Guid fileId = item.Result.WorkId;
            Guid cancelId = await PostCompensationEndpointAddress<Guid>();
            Console.WriteLine("Compensate file {0} - confirmation Id: {1}.", fileId, cancelId);

            return true;
        }

        public override Uri WorkItemEndpointAddress => new Uri("http://localhost:51246/Create");

        public override Uri CompensationEndpointAddress => new Uri("http://localhost:51246/Cancel");
    }
}