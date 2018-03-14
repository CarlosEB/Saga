using Saga.Workers.WorkLog.Base;

namespace Saga.Workers.WorkLog
{
    public class MaskFileWorkResult : WorkResultBase
    {
        public string MaskedFileName { get; set; }
    }
}