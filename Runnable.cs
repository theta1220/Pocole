using System.Collections.Generic;

namespace Pocole
{
    public abstract class Runnable
    {
        public string Source { get; protected set; } = "";
        public int ExecuteCount { get; private set; } = 0;
        public List<Runnable> Runnables { get; private set; } = new List<Runnable>();
        public Block Parent { get; private set; } = null;

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
                return false;
            }
            Runnables[ExecuteCount].Run();
            if (!Runnables[ExecuteCount].Execute())
            {
                ExecuteCount++;
            }
            return true;
        }

        protected abstract void Run();

        protected void RunningLog()
        {
            Log.Info("{0}: ExecuteCount:{1}/{2}", GetType().Name, ExecuteCount, Runnables.Count);
            Log.Info("{0}", Source);
        }

        public bool IsCompleted()
        {
            return ExecuteCount >= Runnables.Count;
        }
    }
}