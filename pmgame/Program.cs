using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmgame
{

    /// <summary>
    /// In the world of software through the currency of time we build tools.
    /// 
    /// 
    /// First Iteration
    /// I want a worker completing a list of tasks.
    /// I need a main loop which iterates at an atomic timestep (1min)
    /// I want
    /// </summary>


    class Program
    {
        List<Worker> workers = new List<Worker>();
        TaskList allTasks = new TaskList();


        static void Main(string[] args)
        {
            var dis = new Program();
            dis.workers = new List<Worker>();
            dis.allTasks = new TaskList();
            GenerateLoad(dis);

            dis.MainLoop();
        }

        private static void GenerateLoad(Program dis)
        {
            for (int i = 0; i < 10; i++)
            {
                var guy = new Worker();
                dis.workers.Add(guy);
            }


            for (int j = 0; j < 50; j++)
            {
                var task = new Task();
                for (int i = 0; i < 50; i++)
                {
                    task.process.Add(new Action() { untilComplete = TimeSpan.FromMinutes(5), isAutomated = false });
                }
                dis.allTasks.Add(task);
            }

            // split tasks

            var groups = dis.allTasks.GroupBy(task => task.GetHashCode() % dis.workers.Count());
            foreach (var group in groups)
            {
                dis.workers[group.Key].personalTaskList = new TaskList(group.ToList());
            }
        }

        public void MainLoop()
        {
            DateTime simTime = DateTime.Now;
            TimeSpan atomicTimestep = TimeSpan.FromSeconds(60);

            Console.WriteLine("Start: " + simTime.ToString("yyyy-MM-dd hh:mm:ss"));
            while( allTasks.Any(t=>! t.complete) )
            {
                foreach (var worker in workers)
                {
                    worker.Work(simTime, atomicTimestep);
                }

                // Update Time
                simTime = simTime.Add(atomicTimestep);
            }

            Console.WriteLine("End: " + simTime.ToString("yyyy-MM-dd hh:mm:ss"));

        }
    }

}
