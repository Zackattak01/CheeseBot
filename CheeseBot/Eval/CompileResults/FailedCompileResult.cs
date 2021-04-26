using System.Collections.Generic;
using Microsoft.CodeAnalysis.Emit;

namespace CheeseBot.Eval
{
    public class FailedCompileResult : ICompileResult
    {
        public EmitResult CompilationResult { get; }

        public IEnumerable<string> Errors => FormatErrors(CompilationResult);

        public FailedCompileResult(EmitResult compilationResult)
        {
            CompilationResult = compilationResult;
        }

        private static IEnumerable<string> FormatErrors(EmitResult compilationResult)
        {
            foreach (var codeIssue in compilationResult.Diagnostics)
            {
                var issue = $"ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()}, " +
                            $"Location: {codeIssue.Location.GetLineSpan()}, " +
                            $"Severity: {codeIssue.Severity}";

                yield return issue;
            }
        }
    }
}