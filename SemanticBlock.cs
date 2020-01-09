namespace Pocole
{
    public enum SemanticType
    {
        Process,
    }

    public abstract class SemanticBlock : Runnable
    {
        public SemanticType SemanticType { get; private set; }
        public Block Block { get; private set; }

        public bool Initialize(Runnable parent, string source, SemanticType type)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }
            SemanticType = type;
            return true;
        }

        public void AddBlock(Block block)
        {
            Block = block;
            Runnables.Add(block);
        }
    }
}