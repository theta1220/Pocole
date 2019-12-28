using System.Collections.Generic;

namespace Pocole
{
    public abstract class Runnable
    {
        public string Source { get; protected set; } = "";
        public int ExecuteCount { get; private set; } = 0;
        public List<Runnable> Runnables { get; private set; } = new List<Runnable>();
        public Block Parent { get; private set; } = null;

        private bool _isActivated = false;

        public bool Initialize(Block parent, string source)
        {
            Parent = parent;
            Source = source;
            return true;
        }

        public virtual bool Execute()
        {
            if (IsCompleted())
            {
                RunningLog();
                return false;
            }
            if (ExecuteCount == 0 && !_isActivated)
            {
                _isActivated = true;
                Activate();
            }
            Runnables[ExecuteCount].Run();
            if (!Runnables[ExecuteCount].Execute())
            {
                Runnables[ExecuteCount].ExecuteCount = 0;
                Runnables[ExecuteCount]._isActivated = false;
                Runnables[ExecuteCount].Finalize();
                ExecuteCount++;
            }
            return true;
        }

        protected abstract void Run();

        protected virtual void Activate()
        {
        }

        protected void Finalize()
        {
            foreach (var run in Runnables)
            {
                run.Finalize();
            }
        }

        protected void RunningLog()
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