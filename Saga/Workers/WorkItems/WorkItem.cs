//using Saga.Processes.Base;
//using Saga.Workers.Base;
//using Saga.Workers.Interfaces;

//namespace Saga.Workers.WorkItems
//{
//    public class WorkItem<TProcess> : WorkItemBase where TProcess : ProcessBase, new()
//    {
//        public WorkItem(IWorkItemArguments args) : base(args)
//        {
//        }

//        public override ProcessBase CreateProcess()
//        {
//            return new TProcess();
//        }
//    }
//}