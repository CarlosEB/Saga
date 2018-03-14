using Saga.Workers.WorkLog.Base;

namespace Saga.Workers.WorkLog
{
    public class TokenFileWorkResult : WorkResultBase
    {
        public string UntokenizedFileName { get; set; }
    }
}