using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Interfaces;
using Saga.Processes.Base;
using Saga.Workers.Base;
using Saga.Workers.WorkLog;

namespace Saga.Processes
{
    internal class ValidateFileProcess : ProcessBase
    {
        public override async Task<WorkLog> DoWorkAsync(IList<WorkLog> workLogs, WorkItemBase workItem = null)
        {
            MapFileWorkResult workResultMap = GetWorkLog<MapFileWorkResult>(workLogs);
            MapSendLogWorkResult workResultMapSendLog = GetWorkLog<MapSendLogWorkResult>(workLogs);

            Console.WriteLine($"Validate Map File - ClearingFileId: {workResultMap.ClearingFileId}");
            Console.WriteLine($"Validate Map Send Log - SendLogId: {workResultMapSendLog.SendLogId}");

            await Task.Delay(TimeSpan.FromSeconds(3));
            if (new Random().Next(1, 100) < 50) throw new Exception("ValidateFileProcess");

            Guid id = Guid.NewGuid();
            Console.WriteLine("Validate file Id: {0}", id);

            ValidateFileWorkResult result = new ValidateFileWorkResult
            {
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
            Console.WriteLine("Compensate Validate file {0} - confirmation Id: {1}.", fileId, cancelId);

            return true;

        }

        public override Uri WorkItemEndpointAddress => new Uri("orchestrator://ValidateFileProcess/Work");

        public override Uri CompensationEndpointAddress => new Uri("orchestrator://ValidateFileProcess/Cancel");
    }
}