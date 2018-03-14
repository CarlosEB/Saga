using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Workers.Base;

namespace Saga.Interfaces
{
    public interface IRoutingSlip
    {       
        bool IsCompleted { get; }

        bool IsInProgress { get; }

        bool IsAborted { get; set; }

        Task<bool> ProcessNextAsync();

        Task<bool> UndoLastAsync();

        Uri ProgressUri { get; set; }

        Uri CompensationUri { get; }

        void SetWorkItems(IList<WorkItemBase> workItems);

        string BrokenStep { get; set; }
    }
}