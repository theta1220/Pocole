using System.Collections.Generic;
using Sumi.Util;
using System;
using System.Linq;

namespace Sumi
{
    public class Foreach : LoopBlock
    {
        public string ValueName { get; private set; }
        public string ArrayName { get; private set; }
        public string CountName { get; private set; }
        public int Count;

        private Value targetValue = null;
        private List<Value> targetArray = null;
        private Value countValue = null;

        private bool executedInitSource = false;

        public Foreach(Runnable parent, string source) : base(parent, source)
        {
            var split = source.PoRemove(' ').PoExtract('(', ')').PoSplit(':');
            ValueName = split[0];
            ArrayName = split[1];
            if (split.Length > 2)
            {
                CountName = split[2];
            }
            Count = 0;
            executedInitSource = false;
        }

        public Foreach(Foreach other) : base(other)
        {
            ValueName = other.ValueName;
            ArrayName = other.ArrayName;
            CountName = other.CountName;
            Count = other.Count;
            executedInitSource = other.executedInitSource;
        }

        public override Runnable Clone() { return new Foreach(this); }

        private void Initialize()
        {
            executedInitSource = true;
            Count = 0;
            InitializeValues();
        }

        private void InitializeValues()
        {
            targetArray = Calc.Execute(GetParentBlock(), ArrayName, typeof(object)).Object as List<Value>;
            Log.Assert(targetArray != null, "配列が見つかりませんでした:{0}", ArrayName);

            targetValue = new Value(ValueName);
            AddValue(targetValue);

            if (CountName != "")
            {
                countValue = new Value(CountName, 0);
                AddValue(countValue);
            }
        }

        public override void OnEntered()
        {
            if (!executedInitSource)
            {
                Initialize();
            }

            if (targetArray.Count == 0 || targetArray.Count <= Count)
            {
                SkipExecute();
                return;
            }

            PickValue();
        }

        public override void OnLeaved()
        {
            base.OnLeaved();
            Count++;

            if (Count >= targetArray?.Count)
            {
                IsContinuous = false;
            }
            InitializeValues();
        }

        public override void OnReset()
        {
            base.OnReset();
            executedInitSource = false;
            targetValue = null;
            countValue = null;
            targetArray = null;
            Count = 0;
        }

        private void PickValue()
        {
            targetValue.Object = targetArray[Count].Object;
            if (countValue != null)
            {
                countValue.Object = Count;
            }
        }
    }
}