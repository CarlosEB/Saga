using System;

namespace Saga.Workers.Interfaces
{
    public interface ICreateFileWorkItemArguments : IWorkItemArguments
    {
        DateTime SendDate { get; set; }
    }
}