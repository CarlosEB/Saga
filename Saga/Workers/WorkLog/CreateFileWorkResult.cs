using Saga.Workers.WorkLog.Base;

namespace Saga.Workers.WorkLog
{
    public class CreateFileWorkResult : WorkResultBase
    {
        public string FileName { get; set; }
    }
}