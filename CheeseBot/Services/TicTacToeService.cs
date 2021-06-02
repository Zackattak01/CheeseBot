using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Extensions;
using CheeseBot.Games;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Disqord.Models;
using Disqord.Rest;
using Disqord.Rest.Api;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Serilog;

namespace CheeseBot.Services
{
    public class TicTacToeService : DiscordBotService
    {
        private readonly Dictionary<Snowflake, TicTacToeGame> _gameDictionary;

        public TicTacToeService(ILogger<TicTacToeService> logger, DiscordBotBase bot) : base(logger, bot)
        {
            _gameDictionary = new Dictionary<Snowflake, TicTacToeGame>();
        }
        
        public void AddGame(TicTacToeGame game)
        {
            _gameDictionary.Add(game.MessageId, game);
        }

        protected override async ValueTask OnInteractionReceived(InteractionReceivedEventArgs e)
        {
            if (e.Interaction is not IComponentInteraction componentInteraction)
                return;

            if (!_gameDictionary.TryGetValue(componentInteraction.Message.Id, out var game))
                return;

            if (!game.IsAccepted)
                await HandleAcceptGame(componentInteraction, game);
            else
                await HandleGameMove(componentInteraction, game);

        }

        private async Task HandleAcceptGame(IComponentInteraction componentInteraction, TicTacToeGame game)
        {
            if (componentInteraction.Author.Id != game.OpponentId)
            {
                await componentInteraction.DeferMessageUpdateAsync();
                return;
            }

            switch (componentInteraction.ComponentId)
            {
                case TicTacToeGame.AcceptId:
                    game.Accept();

                    var actionRows = game.GenerateGameBoardComponent();
                        
                    await componentInteraction.RespondWithMessageUpdateAsync(new LocalMessage()
                        .WithContent($"{Mention.User(game.ChallengerId)} accepted your challenge!  {Mention.User(game.CurrentPlayerId)}'s move!")
                        .WithComponents(actionRows)
                        .WithAllowedMentions(LocalAllowedMentions.None));
                    break;
                case TicTacToeGame.DenyId:
                    _gameDictionary.Remove(componentInteraction.Message.Id);
                    
                    await componentInteraction.RespondWithMessageUpdateAsync(new LocalMessage()
                        .WithContent($"{Mention.User(game.OpponentId)} denied your challenge!")
                        .WithAllowedMentions(LocalAllowedMentions.None));
                    break;
            }
        }

        private async Task HandleGameMove(IComponentInteraction componentInteraction, TicTacToeGame game)
        {
            if (!int.TryParse(componentInteraction.ComponentId, out var boardPos) ||
                boardPos is < 0 or > 8 ||
                (componentInteraction.Author.Id != game.ChallengerId && componentInteraction.Author.Id != game.OpponentId))
            {
                await componentInteraction.DeferMessageUpdateAsync();
                return;
            }

            if (game.MakeMove(componentInteraction.Author.Id, boardPos))
            {
                string messageContents;
                if (game.CheckGameOver(out var winner))
                {
                    _gameDictionary.Remove(game.MessageId);
                    messageContents = winner != default ? $"Game over! {Mention.User(winner)} won!" : $"Game over! Cats game!";
                }
                else
                    messageContents = $"{Mention.User(game.CurrentPlayerId)}'s move!";
                
                var actionRows = game.GenerateGameBoardComponent();
                await componentInteraction.RespondWithMessageUpdateAsync(new LocalMessage()
                    .WithContent(messageContents)
                    .WithAllowedMentions(LocalAllowedMentions.None)
                    .WithComponents(actionRows));
            }
            else
                await componentInteraction.DeferMessageUpdateAsync();
        }
    }
}