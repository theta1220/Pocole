namespace Pocole
{
    public enum SemanticType
    {
        Process,
        MethodDeclarer,
    }

    public abstract class SemanticBlock : Runnable
    {
        public SemanticType SemanticType { get; private set; }
        public Block Block { get; set; }

        public bool Initialize(Block parent, string source, SemanticType type)
        {
            if (!base.Initialize(parent, source)) { Log.InitError(); return false; }
            SemanticType = type;
            return true;
        }
    }
}