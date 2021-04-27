using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CheeseBot.Eval
{
    // A lot of memory is being allocated when compiling (somewhere between 40 to 80mb PER compilation)
    // This memory is not being reclaimed by the GC
    // At first I thought it was memory leak, but it stops "leaking" memory at around 1300-1400mb
    // Are these caches being built up by the compiler?
    public static class CompileUtils
    {
        private static IEnumerable<MetadataReference> _references = null;
        
        public static ICompileResult CompileCommandModule(string moduleName, string code)
        {
            var stringBuilder = new StringBuilder();

            foreach (var nameSpace in EvalUtils.EvalNamespaces)
                stringBuilder.Append($"using {nameSpace};");
            
                
            stringBuilder.Append(code);

            return CompileAndLoadAssembly(moduleName, stringBuilder.ToString());
        }

        private static ICompileResult CompileAndLoadAssembly(string assemblyName, string code)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(code);

            var compilation = CSharpCompilation.Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(GetReferences())
                .AddSyntaxTrees(tree);

            using var stream = new MemoryStream();
            var compilationResult = compilation.Emit(stream);
            

            if (compilationResult.Success)
            {
                stream.Seek(0, SeekOrigin.Begin);

                var alc = new UnloadableAssemblyLoadContext();
                alc.LoadFromStream(stream);
                return new SuccessfulCompileResult(alc, compilationResult);
            }

            return new FailedCompileResult(compilationResult);
        }

        private static IEnumerable<MetadataReference> GetReferences()
        {
            if (_references is not null)
                return _references;
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location));

            _references = assemblies.Select(assembly => MetadataReference.CreateFromFile(assembly.Location));
            return _references;
        }
    }
}