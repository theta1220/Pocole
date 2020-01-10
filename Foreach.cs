using System.Collections.Generic;

namespace Pocole
{
    [System.Serializable]
    public class Foreach : LoopBlock
    {
        public string ValueName { get; private set; }
        public string ArrayName { get; private set; }
        public int Count;

        private Value targetValue = null;
        private List<Value> targetArray = null;

        private bool executedInitSource = false;

        public new bool Initialize(Runnable parent, string source)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }

            var split = Util.String.Split(Util.String.Extract(Util.String.Remove(source, ' '), '(', ')'), ':');
            ValueName = split[0];
            ArrayName = split[1];
            Count = 0;
            executedInitSource = false;

            return true;
        }

        public override void OnEntered()
        {
            if (!executedInitSource)
            {
                executedInitSource = true;

                targetArray = FindValue(ArrayName).Object as List<Value>;

                targetValue = new Value();
                if (!targetValue.Initialize(ValueName)) { Log.InitError(); return; }
                AddValue(targetValue);
            }

            PickValue();

            if (Count >= targetArray.Count)
            {
                IsContinuous = false;
            }
        }

        private void PickValue()
        {
            targetValue.SetValue(targetArray[Count].Object);
            Count++;
        }
    }
}