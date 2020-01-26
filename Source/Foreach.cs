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

        public override void OnEntered()
        {
            if (!executedInitSource)
            {
                executedInitSource = true;

                targetArray = Calc.Execute(GetParentBlock(), ArrayName, typeof(object)).Object as List<Value>;
                if (targetArray == null)
                {
                    Log.Error("配列が見つかりませんでした:{0}", ArrayName);
                    throw new System.Exception("array not found.");
                }

                targetValue = new Value(ValueName);
                AddValue(targetValue);

                if (CountName != "")
                {
                    countValue = new Value(CountName, 0);
                    AddValue(countValue);
                }
            }

            PickValue();
        }

        public override void OnLeaved()
        {
            Count++;

            if (Count >= targetArray.Count)
            {
                IsContinuous = false;
            }
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