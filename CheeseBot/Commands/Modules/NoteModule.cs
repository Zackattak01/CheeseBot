using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Disqord;
using CheeseBot.EfCore;
using CheeseBot.EfCore.Entities;
using CheeseBot.Extensions;
using Disqord;
using Disqord.Bot;
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
        [Description("Takes a note.  I'll hold it for you since I know your forgetful.")]
        public async Task<DiscordCommandResult> CreateNoteAsync(string content)
        {
            var note = new Note(Context.Author.Id, content, DateTime.Now);
            await _dbContext.AddAsync(note);
            await _dbContext.SaveChangesAsync();
            return Response("Note taken!");
        }
        
        [Command("list", "")]
        [Description("Lists all your notes.  In a nice format just because I'm a good guy.")]
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
                    var e = new LocalEmbed().WithDefaultColor();

                    foreach (var note in notes)
                        e.AddField($"Note {note.Id}" , note.ToString());
                    
                    var content = $"You have {notes.Count} {(notes.Count == 1 ? "note" : "notes")}";
                    return Response(content, e);
                }
                default:
                {
                    var fieldBuilders = new List<LocalEmbedField>(notes.Count);

                    foreach (var note in notes)
                    {
                        fieldBuilders.Add(new LocalEmbedField().WithName($"Note {note.Id}").WithValue(note.ToString()));
                    }

                    var config = FieldBasedPageProviderConfiguration.Default.WithContent($"You have {notes.Count} notes");
                    return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                }
            }
        }
        
        [Command("delete", "remove")]
        [Description("Removes a reminder.")]
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
        [Description("Edits a note because we know you messed about the first time.")]
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