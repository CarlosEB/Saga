using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Interfaces;
using Saga.Processes.Base;
using Saga.Workers.Base;
using Saga.Workers.WorkLog;

namespace Saga.RoutingSlip
{
    public class RoutingSlip : IRoutingSlip
    {
        private readonly Stack<WorkLog> _doneWorkLogs = new Stack<WorkLog>();
        private readonly Queue<WorkItemBase> _nextWorkItem = new Queue<WorkItemBase>();

        private readonly IList<WorkLog> _completedWorkLogs = new List<WorkLog>();

        public void SetWorkItems(IList<WorkItemBase> workItems)
        {
            this._nextWorkItem.Clear();
            this._doneWorkLogs.Clear();

            this._hasExceptionOccured = false;

            bool canIncludeWorkItem = false;

            foreach (WorkItemBase workItem in workItems)
            {
                if (workItem.CreateProcess().WorkItemEndpointAddress == ProgressUri)
                    canIncludeWorkItem = true;

                if (canIncludeWorkItem)
                    this._nextWorkItem.Enqueue(workItem);
            }
        }

        public bool IsCompleted => this._nextWorkItem.Count == 0 && this._hasExceptionOccured == false;

        public bool IsAborted { get; set; }

        public bool IsInProgress => this._doneWorkLogs.Count > 0;

        public string BrokenStep { get; set; }

        public Uri ProgressUri { get; set; }

        public Uri CompensationUri => !IsInProgress ? null : this._doneWorkLogs.Peek().GetProcess().CompensationEndpointAddress;

        private bool _hasExceptionOccured;

        public async Task<bool> ProcessNextAsync()
        {
            if (this.IsCompleted)
            {
                throw new InvalidOperationException();
            }

            WorkItemBase currentItem = this._nextWorkItem.Dequeue();
            ProcessBase process = currentItem.CreateProcess();

            return await DoWorkAsync(process, currentItem);
        }

        private async Task<bool> DoWorkAsync(ProcessBase process, WorkItemBase currentItem)
        {
            try
            {
                WorkLog workLog = await process.DoWorkAsync(this._completedWorkLogs, currentItem);

                // Processo executado mas retorno identifica que o resultado não obteve sucesso, Aborta o fluxo.
                if (workLog.Result.Success == false)
                {
                    this.IsAborted = true;
                    return CaptureBrokenStep(process, "Cancel Step Based on result.");
                }

                this._doneWorkLogs.Push(workLog);
                this._completedWorkLogs.Add(workLog);

                // Próximo processo será executado
                return true;
            }
            catch (Exception e)
            {
                this._hasExceptionOccured = true;
                return CaptureBrokenStep(process, e.Message);
            }
        }

        public async Task<bool> UndoLastAsync()
        {
            if (!this.IsInProgress)
            {
                throw new InvalidOperationException();
            }

            WorkLog currentItem = this._doneWorkLogs.Pop();
            ProcessBase process = currentItem.GetProcess();

            try
            {
                return await process.CompensateAsync(currentItem, this);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e.Message);
                throw;
            }
        }

        private bool CaptureBrokenStep(ProcessBase process, string message)
        {
            this.ProgressUri = process.WorkItemEndpointAddress;
            BrokenStep = process.GetType().Name;
            Console.WriteLine(new string('-', 21 + message.Length));
            Console.WriteLine("Broken step Warning: {0}", message);
            Console.WriteLine(new string('-', 21 + message.Length));

            return false;
        }
    }
}