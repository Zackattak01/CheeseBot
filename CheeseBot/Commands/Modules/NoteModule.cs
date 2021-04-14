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
    [Group("note", "notes")]
    public class NoteModule : DiscordModuleBase
    {
        private readonly CheeseBotDbContext _dbContext;
        
        public NoteModule(CheeseBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [Command("", "create")]
        public async Task<DiscordCommandResult> CreateNoteAsync(string content)
        {
            var note = new Note(Context.Author.Id, content, DateTime.Now);
            await _dbContext.AddAsync(note);
            await _dbContext.SaveChangesAsync();
            return Response("Note taken!");
        }
        
        [Command("list", "")]
        public async Task<DiscordCommandResult> ListNotesAsync()
        {
            var notes = await _dbContext.Notes.AsNoTracking()
                .Where(x => x.OwnerId == Context.Author.Id)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

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
        
        [Command("delete", "remove")]
        public async Task<DiscordCommandResult> DeleteNoteAsync(int noteId)
        {
            var note = await _dbContext.Notes.FindAsync(noteId);

            if (note is null)
                return Response("A note with that id does not exist");
            else if (note.OwnerId != Context.Author.Id)
                return Response("You cannot delete other peoples reminders!");

            _dbContext.Notes.Remove(note);
            await _dbContext.SaveChangesAsync();
            return Response("Note deleted!");
        }
        
        [Command("edit")]
        public async Task<DiscordCommandResult> EditNoteAsync(int noteId, [Remainder] string newContent)
        {
            var note = await _dbContext.Notes.FindAsync(noteId);

            if (note is null)
                return Response("A note with that id does not exist");
            else if (note.OwnerId != Context.Author.Id)
                return Response("You cannot edit other peoples reminders!");

            note.Content = newContent;
            
            await _dbContext.SaveChangesAsync();
            return Response("Note edited!");
        }
    }
}