using Saga.Processes.Base;
using Saga.Workers.Interfaces;

namespace Saga.Workers.Base
{
    public abstract class WorkItemBase
    {
        public RoutingSlip.RoutingSlip RoutingSlip { get; set; }

        public IWorkItemArguments Arguments { get; set; }

        public abstract ProcessBase CreateProcess();

        protected WorkItemBase(IWorkItemArguments arguments)
        {
            this.Arguments = arguments;
        }
    }
}