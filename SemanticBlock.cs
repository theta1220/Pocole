namespace Pocole
{
    public enum SemanticType
    {
        Process,
        For,
    }

    [System.Serializable]
    public abstract class SemanticBlock : Runnable
    {
        public SemanticType SemanticType { get; private set; }
        public Block Block { get; private set; }

        public SemanticBlock(Runnable parent, string source, SemanticType type) : base(parent, source)
        {
            SemanticType = type;
        }

        public void AddBlock(Block block)
        {
            Block = block;
            Runnables.Add(block);
        }
    }
}