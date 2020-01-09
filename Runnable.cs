using System.Collections.Generic;

namespace Pocole
{
    public abstract class Runnable
    {
        public string Source { get; protected set; } = "";
        public int ExecuteCount { get; private set; } = 0;
        public List<Runnable> Runnables { get; private set; } = new List<Runnable>();
        public Runnable Parent { get; private set; } = null;

        private bool _isEntered = false;

        public bool Initialize(Runnable parent, string source)
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
            if (Runnables.Count > 0 && Runnables.Count > ExecuteCount && !Runnables[ExecuteCount].Execute())
            {
                ExecuteCount++;
            }
            if (IsCompleted())
            {
                OnLeave();
                return false;
            }
            return true;
        }

        public void ForceExecute()
        {
            while (Execute()) { }
        }

        protected virtual void Run() { }
        public virtual void OnEntered() { }
        public virtual void OnLeaved() { }

        private void ExecuteRun()
        {
            RunningLog();
            Run();
        }

        private void OnEnter()
        {
            _isEntered = true;
            OnEntered();

            // Log.Warn("    {0}---->{1}", GetIndent(), GetType().Name);
        }

        private void OnLeave()
        {
            _isEntered = false;
            OnLeaved();
            ExecuteCount = 0;
            // Log.Warn("    {0}<----{1}", GetIndent(), GetType().Name);
        }

        private void RunningLog()
        {
            // Log.Debug("{0}{1}::{2}/{3}::{4}", GetIndent(), GetType().Name, ExecuteCount, Runnables.Count, Source.Substring(0, System.Math.Clamp(Source.Length, 0, 30)).Replace("\n", ""));
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

        public Block GetParentBlock()
        {
            if (Parent == null) { return null; }
            if (Parent is Block)
            {
                return (Block)Parent;
            }
            return Parent.GetParentBlock();
        }

        public MethodDeclarer GetParentMethod()
        {
            if (Parent == null) { return null; }
            if (Parent is MethodDeclarer)
            {
                return (MethodDeclarer)Parent;
            }
            return Parent.GetParentMethod();
        }
    }
}