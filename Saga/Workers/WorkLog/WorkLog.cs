using Saga.Processes.Base;
using Saga.Workers.Interfaces;

namespace Saga.Workers.WorkLog
{
    public class WorkLog
    {
        private readonly ProcessBase _process;

        public IWorkResult Result { get; }

        public ProcessBase GetProcess()
        {
            return this._process;
        }

        public WorkLog(ProcessBase process, IWorkResult result)
        {
            this.Result = result;
            this._process = process;
        }
    }
}