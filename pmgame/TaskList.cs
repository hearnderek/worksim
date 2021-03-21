using System.Collections.Generic;

namespace pmgame
{
    public class TaskList : List<Task>
    {
        public TaskList()
            : base()
        {

        }

        public TaskList(IEnumerable<Task> tasks)
            : base(tasks)
        {

        }
    }

}
