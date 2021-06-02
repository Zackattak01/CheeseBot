using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Games;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class GameModule : DiscordGuildModuleBase
    {
        private readonly TicTacToeService _ticTacToeService;
        private readonly Random _random;

        public GameModule(TicTacToeService ticTacToeService, Random random)
        {
            _ticTacToeService = ticTacToeService;
            _random = random;
        }

        [Command("tictactoe")]
        public async Task ButtonAsync(IMember opponent)
        {
            if (opponent.Id == Context.Author.Id)
            {
                await Response("You can't challenge yourself!");
                return;
            }

            var localMsg = new LocalMessage()
                .WithContent($"{opponent.Mention} you have been challenged by {Context.Author.Mention}! Do you accept?");

            if (Context.Message.MentionedUsers.All(x => x.Id != opponent.Id))
                localMsg.WithAllowedMentions(new LocalAllowedMentions().WithUserIds(opponent.Id));
            else
                localMsg.WithAllowedMentions(LocalAllowedMentions.None);

            localMsg.AddComponent(new LocalRowComponent().WithComponents(new[]
            {
                new LocalButtonComponent
                {
                    CustomId = TicTacToeGame.AcceptId,
                    Label = "Accept",
                    Style = ButtonComponentStyle.Success
                },
                new LocalButtonComponent
                {
                    CustomId = TicTacToeGame.DenyId,
                    Label = "Deny",
                    Style = ButtonComponentStyle.Danger
                }
            }));

            var msg = await Response(localMsg);
            _ticTacToeService.AddGame(new TicTacToeGame(msg.Id, Context.Author.Id, opponent.Id, _random));
        }
    }
}