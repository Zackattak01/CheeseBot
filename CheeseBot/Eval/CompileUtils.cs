using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CheeseBot.Eval
{

    public static class CompileUtils
    {
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

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location));

            var compilation = CSharpCompilation.Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(assemblies.Select(assembly => MetadataReference.CreateFromFile(assembly.Location)))
                .AddSyntaxTrees(tree);


            var stream = new MemoryStream();
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
    }
}