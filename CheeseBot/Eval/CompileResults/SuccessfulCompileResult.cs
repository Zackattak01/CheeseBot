using System.Runtime.Loader;
using Microsoft.CodeAnalysis.Emit;

namespace CheeseBot.Eval
{
    public class SuccessfulCompileResult : ICompileResult
    {
        public AssemblyLoadContext AssemblyLoadContext { get; }
        public EmitResult CompilationResult { get; }

        public SuccessfulCompileResult(AssemblyLoadContext assemblyLoadContext, EmitResult compilationResult)
        {
            AssemblyLoadContext = assemblyLoadContext;
            CompilationResult = compilationResult;
        }
    }
}