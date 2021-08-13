using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Tasks manager.
    /// </summary>
    public class TasksManager
    {
        private readonly List<Task> _tasks = new List<Task>();
        private readonly object _tasksLock = new object();




        /// <summary>
        /// Run task on main thread.
        /// </summary>        
        public void RunTaskOnMainThread(Task task)
        {
            lock (_tasksLock)
                _tasks.Add(task);
        }

        /// <summary>
        /// Run main thread tasks.
        /// </summary>
        public void RunMainThreadTasks()
        {
            lock (_tasksLock)
            {
                foreach (Task task in _tasks)
                    task.RunSynchronously();

                _tasks.Clear();
            }
        }
    }
}
