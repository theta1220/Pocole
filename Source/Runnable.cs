using System;
using System.Collections.Generic;
using System.Linq;

namespace Sumi
{
    public abstract class Runnable
    {
        public string Source { get; protected set; } = "";
        public int ExecuteCount { get; private set; } = 0;
        public List<Runnable> Runnables { get; private set; } = new List<Runnable>();
        public Runnable Parent { get; set; } = null;

        private bool _isEntered = false;

        public Runnable(Runnable parent, string source)
        {
            Parent = parent;
            Source = source;
        }

        public Runnable(Runnable other)
        {
            Source = other.Source;
            ExecuteCount = other.ExecuteCount;
            foreach (var obj in other.Runnables)
            {
                var clone = (obj as Runnable).Clone();
                clone.Parent = this;
                Runnables.Add(clone);
            }
            Parent = other.Parent;
            _isEntered = other._isEntered;
        }

        public abstract Runnable Clone();

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
                if (!CheckContinue())
                {
                    return false;
                }
                else ResetExecute();
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
        public virtual bool CheckContinue() { return false; }

        private void ExecuteRun()
        {
            RunningLog();
            Run();
        }

        private void OnEnter()
        {
            // Log.Warn("    {0}---->{1}", GetIndent(), GetType().Name);
            _isEntered = true;
            OnEntered();
        }

        private void OnLeave()
        {
            // Log.Warn("    {0}<----{1}", GetIndent(), GetType().Name);
            _isEntered = false;
            OnLeaved();
            ExecuteCount = 0;
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

        public Function GetParentMethod()
        {
            if (Parent == null) { return null; }
            if (Parent is Function)
            {
                return (Function)Parent;
            }
            return Parent.GetParentMethod();
        }

        public Class GetParentClass()
        {
            if (Parent == null) { return null; }
            if (Parent is Class)
            {
                return (Class)Parent;
            }
            return Parent.GetParentClass();
        }

        public LoopBlock GetParentLoopBlock()
        {
            if (Parent == null) { return null; }
            if (Parent is LoopBlock)
            {
                return (LoopBlock)Parent;
            }
            return Parent.GetParentLoopBlock();
        }

        public Block GetRootBlock()
        {
            var parent = GetParentBlock();
            if (parent != null)
            {
                return parent.GetRootBlock();
            }
            return (Block)this;
        }
    }
}