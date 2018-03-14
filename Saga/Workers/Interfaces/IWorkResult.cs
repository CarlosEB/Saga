using System;

namespace Saga.Workers.Interfaces
{
    public interface IWorkResult
    {
        Guid WorkId { get; set; }

        bool Success { get; set; }
    }    
}