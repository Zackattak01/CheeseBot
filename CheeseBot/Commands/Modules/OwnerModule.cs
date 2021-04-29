using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Eval;
using CheeseBot.Extensions;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus.Paged;
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
        private const int ByteToMbConversionFactor = 1000000;
        
        private readonly CommandModuleCompilingService _commandModuleCompilingService;
        public OwnerModule(CommandModuleCompilingService commandModuleCompilingService)
        {
            _commandModuleCompilingService = commandModuleCompilingService;
        }


        [Command("shutdown", "stop", "die", "kill", "exit")]
        [Description("Shuts down and does not restart it")]
        public void Shutdown()
            => Environment.Exit(0);
        
        
        [Command("restart", "update")]
        [Description("Shuts down and does restarts it")]
        public void Restart()
            => Environment.Exit(1);

        [Command("mem", "memory")]
        public DiscordCommandResult Memory()
        {
            using var process = Process.GetCurrentProcess();
            var mbUsed = process.WorkingSet64 / ByteToMbConversionFactor;
            return Response($"I am currently allocated {mbUsed}mb");
        }

        [Command("gc", "collect")]
        public DiscordCommandResult CollectGarbage()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return Response("Garbage collected!");
        }

        [Command("eval", "evaluate")]
        [RunMode(RunMode.Parallel)]
        public async Task<DiscordCommandResult> EvalAsync([Remainder] string code = null)
        {
            if (code == null)
            {
                var messageRef = Context.Message.ReferencedMessage.GetValueOrDefault();
                if (messageRef is not null)
                    code = messageRef.Content;
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

        [Command("load")]
        [RunMode(RunMode.Parallel)]
        public async Task<DiscordCommandResult> Load(Snowflake id)
        {
            var msg = await Context.Bot.FetchMessageAsync(Context.ChannelId, id);

            if (msg is null)
                return Response("A message with that id does not exist");
            
            return Load(id, msg.Content);
        }

        [Command("load")]
        [RunMode(RunMode.Parallel)]
        public DiscordCommandResult Load([Remainder] string code = null)
        {
            var id = Context.Message.Id;
            if (code == null) 
            {
                if (Context.Message.ReferencedMessage.HasValue)
                {
                    code = Context.Message.ReferencedMessage.Value.Content;
                    id = Context.Message.ReferencedMessage.Value.Id;
                }
                else
                    return Response("More code needed, sir.");
            }

            return Load(id, code);
        }

        [Command("unload")]
        public DiscordCommandResult Unload(Snowflake? id = null)
        {
            if (id is null)
            {
                if (Context.Message.ReferencedMessage.HasValue)
                    id = Context.Message.ReferencedMessage.Value.Id;
                else
                    return Response("Please provide an id");
            }

            var result = _commandModuleCompilingService.RemoveModules(id.Value);
            return Response(result ? "Module(s) unloaded" : "No modules are loaded under that id");
        }

        private DiscordCommandResult Load(Snowflake id, string code)
        {
            code = EvalUtils.TrimCode(code);

            if (_commandModuleCompilingService.IsModuleLoaded(id))
                return Response("That code is already loaded.");

            ICompileResult result;
            try
            {
                result = _commandModuleCompilingService.CreateModules(id, code);
            }
            catch (CommandMappingException mappingException)
            {
                return Response(mappingException.Message);
            }
            
            
            switch (result)
            {
                case SuccessfulCompileResult:
                    return Response("Loaded command module(s).");
                case FailedCompileResult failedResult:
                {
                    var errorString = string.Join('\n', failedResult.Errors);
                    if (errorString.Length > LocalMessageBuilder.MAX_CONTENT_LENGTH)
                    {
                        var pages = errorString.SplitInParts(LocalMessageBuilder.MAX_CONTENT_LENGTH);
                        return Pages(pages.Select(x => new Page(x)));
                    }
                    else
                    {
                        return Response($"Command module loading failed:\n {errorString}");
                    }
                }
                default:
                    return Response("The space time continuum is screwed up again.");
            }
        }
    }
}