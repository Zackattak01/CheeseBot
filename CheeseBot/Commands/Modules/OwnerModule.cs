using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Eval;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [RequireBotOwner]
    public class OwnerModule : DiscordModuleBase
    {
        [Command("shutdown", "stop", "die", "kill")]
        [Description("Shuts down and does not restart it")]
        public void Shutdown()
        {  
            //TODO: log out
            //cant find the methods rn
            Environment.Exit(0);
        }
        
        [Command("restart", "update")]
        [Description("Shuts down and does restarts it")]
        public void Restart()
        {  
            //TODO: log out
            //cant find the methods rn
            Environment.Exit(1);
        }
        
        [Command("eval", "evaluate")]
        [RunMode(RunMode.Parallel)]
        public async Task<DiscordCommandResult> Eval([Remainder] string code = null)
        {
            if (code == null) 
            {
                if (Context.Message.ReferencedMessage.HasValue)
                    code = Context.Message.ReferencedMessage.Value.Content;
                else
                    return Response("More code needed, sir.");
            }
            code = EvalUtils.ValidateCode(code);
            var scriptOptions = ScriptOptions.Default
                .WithImports(EvalUtils.EvalNamespaces)
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));
            var script = CSharpScript.Create(code, scriptOptions, typeof(EvalGlobals));
            try
            {
                using (Context.Bot.BeginTyping(Context.ChannelId))
                {
                    var stopwatch = Stopwatch.StartNew();
                    var diagnostics = script.Compile();
                    stopwatch.Stop();
                    if (diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
                    {
                        var eb = new LocalEmbedBuilder()
                         .WithTitle("Compilation Failure")
                        .WithDescription($"{diagnostics.Length} {(diagnostics.Length > 1 ? "errors" : "error")}")
                        .WithColor(Color.Red)
                        .WithFooter($"{stopwatch.Elapsed.TotalMilliseconds}ms");
                         for (var i = 0; i < diagnostics.Length; i++)
                        {
                            if (i > 3)
                                break;
                            var diagnostic = diagnostics[i];
                            var lineSpan = diagnostic.Location.GetLineSpan().Span;
                            eb.AddField($"Error `{diagnostic.Id}` at {lineSpan.Start} - {lineSpan.End}", diagnostic.GetMessage());
                        }
                         
                        if (diagnostics.Length > 4) 
                            eb.AddField($"Skipped {diagnostics.Length - 4} {(diagnostics.Length - 4 > 1 ? "errors" : "error")}", "You should be able to fix it.");
                        return Response(eb);
                    }
                    var globals = new EvalGlobals(Context);
                    var state = await script.RunAsync(globals, _ => true);
                    if (state.Exception != null)
                    {
                        var eb = new LocalEmbedBuilder()
                            .WithTitle("Execution Failure")
                            .WithDescription(state.Exception.ToString())
                            .WithColor(Color.Red)
                            .WithFooter($"{stopwatch.Elapsed.TotalMilliseconds}ms");
                        return Response(eb);
                    }
                    if (state.ReturnValue == null || state.ReturnValue is string value && string.IsNullOrWhiteSpace(value))
                    {
                        return Reaction(new LocalEmoji("✅"));
                    }
                    else
                    {
                        return Response(state.ReturnValue.ToString());
                    }
                }
                
            }
            catch (Exception ex)
            {
                Context.Bot.Logger.LogError(ex, "An unexpected exception occurred when evaluating code.");
                return Response($"An unexpected exception occurred: {ex.Message}.");
            }
}
    }
}