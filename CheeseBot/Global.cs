global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Net.Http;
global using System.Threading.Tasks;
global using CheeseBot.Disqord;
global using CheeseBot.EfCore;
global using CheeseBot.Eval;
global using CheeseBot.Extensions;
global using CheeseBot.Services;
global using CheeseBot.EfCore.Entities;
global using CheeseBot.Scheduling;
global using Disqord;
global using Disqord.Bot;
global using Disqord.Bot.Hosting;
global using Disqord.Gateway;
global using Disqord.Rest;
global using HumanTimeParser.English;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Qmmands;

namespace CheeseBot
{
    public static class Global
    {
        //Color appears the same as the default embed color
        public static readonly Color DefaultEmbedColor = new(0x2F3136);
        
        public const string CheeseBotRepo = "https://github.com/Zackattak01/CheeseBot";

        public const ulong AuthorId = 332675511161585666;
    }
}