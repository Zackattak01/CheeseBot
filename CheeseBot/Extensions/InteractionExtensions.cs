using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Disqord;
using Disqord.Models;
using Disqord.Rest;
using Disqord.Rest.Api;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CheeseBot.Extensions
{
    public static class InteractionExtensions
    {
        public static async Task RespondWithMessageAsync(this IInteraction interaction, LocalMessage message, bool ephemeral = false)
        {
            message.Validate();
            var apiClient = interaction.GetRestClient().ApiClient;
            var model = new CreateInteractionResponseJsonRestRequestContent
            {
                Type = InteractionResponseType.ChannelMessage,
                Data = new InteractionCallbackDataJsonModel
                {
                    Content = Optional.FromNullable(message.Content),
                    Tts = Optional.Conditional(message.IsTextToSpeech, true),
                    Embeds = Optional.Conditional(message.Embed is not null, new[] {message.Embed.ToModel()}),
                    AllowedMentions = Optional.FromNullable(message.AllowedMentions.ToModel()),
                    // Components =
                    Flags = Optional.Conditional(ephemeral, 64)
                }
            };

            if (message.Components.Count != 0)
            {
                model.Data.Value.ExtensionData["components"] =
                    apiClient.Serializer.GetJsonNode(message.Components.Select(x => x.ToModel()).ToArray());
            }

            await apiClient.CreateInitialInteractionResponseAsync(interaction.Id, interaction.Token, model);
        }

        public static async Task RespondWithMessageUpdateAsync(this IInteraction interaction, LocalMessage message)
        {
            // message.Validate();
            var apiClient = interaction.GetRestClient().ApiClient;
            var model = new CreateInteractionResponseJsonRestRequestContent
            {
                Type = InteractionResponseType.MessageUpdate,
                Data = new InteractionCallbackDataJsonModel
                {
                    Content = message.Content,
                    Embeds = Optional.Conditional(message.Embed is not null, new[] {message.Embed.ToModel()}),
                    AllowedMentions = message.AllowedMentions.ToModel()
                }
            };
            
            if (message.Components.Count != 0)
            {
                model.Data.Value.ExtensionData["components"] =
                    apiClient.Serializer.GetJsonNode(message.Components.Select(x => x.ToModel()).ToArray());
            }
            
            var responseModel = await apiClient.CreateInitialInteractionResponseAsync(interaction.Id, interaction.Token, model);
        }
        public static async Task<IUserMessage> FollowupWithMessageAsync(this IInteraction interaction, LocalMessage message, bool ephemeral = false)
        {
            message.Validate();
            var apiClient = interaction.GetRestClient().ApiClient;
            var model = new ExecuteWebhookJsonRestRequestContent()
            {
                Content = Optional.FromNullable(message.Content),
                Tts = Optional.Conditional(message.IsTextToSpeech, true),
                Embeds = Optional.Conditional(message.Embed is not null, new[] {message.Embed.ToModel()}),
                AllowedMentions = Optional.FromNullable(message.AllowedMentions.ToModel()),
                Components =Optional.Conditional(message.Components.Count != 0, x => x.Select(x => x.ToModel()).ToArray(), message.Components)
            };

            // model.Data.Value.ExtensionData["components"] = apiClient.Serializer.GetJsonNode(Optional.Conditional(message.Components.Count != 0,
            //     x => x.Select(x => x.ToModel()).ToArray(), message.Components));

            var responseModel = await apiClient.CreateFollowupInteractionResponseAsync(interaction.ApplicationId, interaction.Token, model);
            return new TransientUserMessage(interaction.Client, responseModel);
        }

        public static async Task DeferMessageUpdateAsync(this IInteraction interaction, bool ephemeral = false)
        {
            var apiClient = interaction.GetRestClient().ApiClient;
            var model = new CreateInteractionResponseJsonRestRequestContent
            {
                Type = InteractionResponseType.DeferredMessageUpdate,
                Data = new InteractionCallbackDataJsonModel
                {
                    Flags = Optional.Conditional(ephemeral, 64)
                }
            };
            await apiClient.CreateInitialInteractionResponseAsync(interaction.Id, interaction.Token, model);
        }
        public static async Task DeferMessageResponseAsync(this IInteraction interaction, bool ephemeral = false)
        {
            var apiClient = interaction.GetRestClient().ApiClient;
            var model = new CreateInteractionResponseJsonRestRequestContent
            {
                Type = InteractionResponseType.DeferredChannelMessage,
                Data = new InteractionCallbackDataJsonModel
                {
                    Flags = Optional.Conditional(ephemeral, 64)
                }
            };
            await apiClient.CreateInitialInteractionResponseAsync(interaction.Id, interaction.Token, model);
        }
    }
}