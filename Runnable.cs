using System.Collections.Generic;

namespace Pocole
{
    public abstract class Runnable
    {
        public string Source { get; protected set; } = "";
        public int ExecuteCount { get; private set; } = 0;
        public List<Runnable> Runnables { get; private set; } = new List<Runnable>();
        public Block Parent { get; private set; } = null;

        private bool _isOnEnterExecuted = false;

        public bool Initialize(Block parent, string source)
        {
            Parent = parent;
            Source = source;
            return true;
        }

        public bool Execute()
        {
            if (IsCompleted())
            {
                RunningLog();
                OnLeave();
                return false;
            }
            if (ExecuteCount == 0 && !_isOnEnterExecuted)
            {
                OnEnter();
            }
            Runnables[ExecuteCount].Run();
            if (!Runnables[ExecuteCount].Execute())
            {
                ExecuteCount++;
            }
            return true;
        }

        protected abstract void Run();
        public virtual void OnEntered() { }
        public virtual void OnLeaved() { }

        public void OnEnter()
        {
            OnEntered();
            _isOnEnterExecuted = true;
            ExecuteCount = 0;
        }

        public void OnLeave()
        {
            OnLeaved();
            _isOnEnterExecuted = false;
            ExecuteCount = 0;
        }

        private void RunningLog()
        {
            // Log.Info("{0}: ExecuteCount:{1}/{2}", GetType().Name, ExecuteCount, Runnables.Count);
            Log.Info("{0}::{1}/{2}::{3}", GetType().Name, ExecuteCount, Runnables.Count, Source);
        }

        public bool IsCompleted()
        {
            return ExecuteCount >= Runnables.Count;
        }
    }
}