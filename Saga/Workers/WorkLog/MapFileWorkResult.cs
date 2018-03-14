using Saga.Workers.WorkLog.Base;

namespace Saga.Workers.WorkLog
{
    public class MapFileWorkResult : WorkResultBase
    {
        public long ClearingFileId{ get; set; }
    }
}