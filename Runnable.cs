using System.Collections.Generic;

namespace Pocole
{
    public abstract class Runnable
    {
        public string Source { get; protected set; } = "";
        public int ExecuteCount { get; private set; } = 0;
        public List<Runnable> Runnables { get; private set; } = new List<Runnable>();
        public Block Parent { get; private set; } = null;

        private bool _isEntered = false;

        public bool Initialize(Block parent, string source)
        {
            Parent = parent;
            Source = source;
            return true;
        }

        public bool Execute()
        {
            if (ExecuteCount == 0 && !_isEntered)
            {
                OnEnter();
            }
            ExecuteRun();
            if (IsCompleted())
            {
                OnLeave();
                return false;
            }
            if (!Runnables[ExecuteCount].Execute())
            {
                ExecuteCount++;
            }
            return true;
        }

        protected virtual void Run() { }
        public virtual void OnEntered() { }
        public virtual void OnLeaved() { }

        public void ExecuteRun()
        {
            RunningLog();
            Run();
        }

        public void OnEnter()
        {
            _isEntered = true;
            OnEntered();

            // Log.Warn("    {0}---->{1}", GetIndent(), GetType().Name);
        }

        public void OnLeave()
        {
            _isEntered = false;
            OnLeaved();
            ExecuteCount = 0;
            // Log.Warn("    {0}<----{1}", GetIndent(), GetType().Name);
        }

        private void RunningLog()
        {
            Log.Debug("{0}{1}::{2}/{3}::{4}", GetIndent(), GetType().Name, ExecuteCount, Runnables.Count, Source.Substring(0, System.Math.Clamp(Source.Length, 0, 30)).Replace("\n", ""));
        }

        public bool IsCompleted()
        {
            return ExecuteCount >= Runnables.Count;
        }

        public void SkipExecute()
        {
            ExecuteCount = Runnables.Count;
        }

        public void ResetExecute()
        {
            ExecuteCount = 0;
        }

        private int ParentCount()
        {
            int count = 0;
            if (Parent != null)
            {
                count++;
                count += Parent.ParentCount();
            }
            return count;
        }

        private string GetIndent()
        {
            var count = ParentCount();
            var indent = "";
            for (var i = 0; i < count; i++) indent += "    ";
            return indent;
        }
    }
}