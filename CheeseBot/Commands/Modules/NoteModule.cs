using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Disqord;
using CheeseBot.EfCore;
using CheeseBot.EfCore.Entities;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Microsoft.EntityFrameworkCore;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class NoteModule : DiscordModuleBase
    {
        private readonly CheeseBotDbContext _dbContext;
        
        public NoteModule(CheeseBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [Command("note")]
        public async Task<DiscordCommandResult> NoteAsync(string content)
        {
            var note = new Note(Context.Author.Id, content, DateTime.Now);
            await _dbContext.AddAsync(note);
            await _dbContext.SaveChangesAsync();
            return Response("Note taken!");
        }
        
        [Command("notes")]
        public async Task<DiscordCommandResult> ListNotesAsync()
        {
            var notes = await _dbContext.Notes.AsNoTracking().Where(x => x.OwnerId == Context.Author.Id).ToListAsync();

            switch (notes.Count)
            {
                case 0:
                    return Response("You have no notes");
                case <= 5:
                {
                    var eb = new LocalEmbedBuilder().WithColor(Global.DefaultEmbedColor);

                    foreach (var note in notes)
                    {
                        eb.AddField($"Note {note.Id}" , note.ToString());
                    }

                    return Response(eb);
                }
                default:
                {
                    var fieldBuilders = new List<LocalEmbedFieldBuilder>(notes.Count);

                    foreach (var note in notes)
                    {
                        fieldBuilders.Add(new LocalEmbedFieldBuilder().WithName($"Note {note.Id}").WithValue(note.ToString()));
                    }
                    
                    return Pages(new FieldBasedPageProvider(fieldBuilders, 5));
                }
            }
        }
    }
}