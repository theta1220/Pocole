
namespace Sumi.Lib
{
    public static class Debug
    {
        public static void PrintBlockTree(Value[] args)
        {
            var block = args[0].Object as Block;
            if (block == null)
            {
                Log.Error("対象がnullでした");
                return;
            }
            block.PrintBlockTree();
        }

        public static void Error(Value[] args)
        {
            var text = args[0].Object as string;
            Log.Error(text);
        }
    }
}