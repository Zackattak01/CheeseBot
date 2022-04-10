using Microsoft.CodeAnalysis.Emit;

namespace CheeseBot.Eval
{
    public interface ICompileResult
    {
        public EmitResult CompilationResult { get; }
    }
}