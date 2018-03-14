using System;
using System.Threading.Tasks;
using Saga.Processes.Base;
using Saga.Workers.Interfaces;

namespace Saga.Interfaces
{
    public interface IBuilder
    {
        IBuilder RunProcess<TProcess>(IWorkItemArguments args = null) where TProcess : ProcessBase, new();

        IBuilder Then();

        Task Start(int startInStep = 1, int times = 0, TimeSpan? retryDelay = null, Action fireIfBrokenStep = null);
    }
}