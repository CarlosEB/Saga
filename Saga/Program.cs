using System;
using Saga.Builder;
using Saga.Interfaces;
using Saga.Processes;
using Saga.Workers.WorkItemArguments;

namespace Saga
{
    internal class Program
    {
        //private static IHostProcess[] _processes;

        private static void Main(string[] args)
        {
            IBuilder flow = FlowFactory.Create()
                .RunProcess<CreateFileProcess>(new CreateFileWorkItemArguments(DateTime.Now))
                .Then().RunProcess<TokenFileProcess>()
                .Then().RunProcess<MaskFileProcess>()
                .Then().RunProcess<MapFileProcess>()
                .Then().RunProcess<SendFileProcess>()
                .Then().RunProcess<MapSendLogProcess>()
                .Then().RunProcess<ValidateFileProcess>()
                .Then().RunProcess<MoveFileProcess>();

            flow.Start(1, 3, TimeSpan.FromSeconds(5), NotifyIfBrokenStep);

            Console.ReadKey();
        }

        private static void NotifyIfBrokenStep()
        {
            Console.WriteLine("**********************");
            Console.WriteLine("Sent NOC notification!");
            Console.WriteLine("**********************");
        }
    }
}