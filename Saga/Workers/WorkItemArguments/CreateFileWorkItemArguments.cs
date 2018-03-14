using System;
using Saga.Workers.Interfaces;

namespace Saga.Workers.WorkItemArguments
{
    public class CreateFileWorkItemArguments : ICreateFileWorkItemArguments
    {
        public DateTime SendDate { get; set; }

        public CreateFileWorkItemArguments(DateTime sendDate)
        {
            this.SendDate = sendDate;
        }
    }
}