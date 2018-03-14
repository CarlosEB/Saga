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
    internal class TokenFileProcess : ProcessBase
    {
        public override async Task<WorkLog> DoWorkAsync(IList<WorkLog> workLogs, WorkItemBase workItem = null)
        {
            CreateFileWorkResult workResult = GetWorkLog<CreateFileWorkResult>(workLogs);

            Console.WriteLine($"Tokenizing file: {workResult.FileName}");

            dynamic id = await RetryHelper.RetryAsync<Exception>(async () => await PostWorkItemEndpointAddress<Guid>(), 3, TimeSpan.FromSeconds(3), Logger);
            Console.WriteLine("Tokenized Id: {0}", id);

            // Simulando um percentual abaixo do esperado para setar como fracasso o passo, mesmo que processado com sucesso (estatística, mínimo esperado não alcançado etc...).
            bool success = new Random().Next(1, 100) < 70;

            TokenFileWorkResult result = new TokenFileWorkResult
            {
                UntokenizedFileName = "GNS.ORG001.T0000001.CS_20180129170710_UnTokenized",
                WorkId = id,
                Success = success
            };

            return new WorkLog(this, result);
        }

        public override async Task<bool> CompensateAsync(WorkLog item, IRoutingSlip routingSlip)
        {
            Guid tokenId = item.Result.WorkId;
            Guid cancelId = await PostCompensationEndpointAddress<Guid>();
            Console.WriteLine("Compensate token {0} - confirmation Id: {1}.", tokenId, cancelId);

            return true;
        }

        public override Uri WorkItemEndpointAddress => new Uri("http://localhost:51246/Token");

        public override Uri CompensationEndpointAddress => new Uri("http://localhost:51246/Cancel");
    }
}