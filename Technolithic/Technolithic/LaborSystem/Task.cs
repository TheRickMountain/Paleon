using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum TaskState
    {
        Success,
        Running,
        Failed,
        Canceled
    }

    public abstract class Task
    {
        public TaskState State { get; set; } = TaskState.Running;

        public CreatureCmp Owner { get; private set; }

        public Task(CreatureCmp owner)
        {
            Owner = owner;
        }

        public abstract void Begin();

        public abstract void BeforeUpdate();

        public TaskState Update()
        {
            switch (State)
            {
                case TaskState.Canceled:
                case TaskState.Failed:
                case TaskState.Success:
                    return State;
                case TaskState.Running:
                    UpdateTask();
                    break;
            }

            return State;
        }

        public abstract void UpdateTask();

        public virtual void Cancel()
        {
            State = TaskState.Canceled;
        }

        public virtual void Complete()
        {

        }

    }
}
