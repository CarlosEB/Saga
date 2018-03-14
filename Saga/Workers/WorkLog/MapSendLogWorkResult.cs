using Saga.Workers.WorkLog.Base;

namespace Saga.Workers.WorkLog
{
    public class MapSendLogWorkResult : WorkResultBase
    {
        public long SendLogId{ get; set; }
    }
}