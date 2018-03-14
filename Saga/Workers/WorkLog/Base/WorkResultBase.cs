using System;
using Saga.Workers.Interfaces;

namespace Saga.Workers.WorkLog.Base
{
    public abstract class WorkResultBase : IWorkResult
    {
        public Guid WorkId { get; set; }

        public bool Success { get; set; }

    }
    
}