using Saga.Workers.WorkLog.Base;

namespace Saga.Workers.WorkLog
{
    public class SendFileWorkResult : WorkResultBase
    {
        public string LogConnectDirect { get; set; }
    }
}