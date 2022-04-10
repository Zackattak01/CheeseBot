﻿// <auto-generated />
using System;
using CheeseBot.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CheeseBot.Migrations
{
    [DbContext(typeof(CheeseBotDbContext))]
    partial class CheeseBotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("CheeseBot.EfCore.Entities.GuildSettings", b =>
                {
                    b.Property<ulong>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<bool>("IsPermitted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_permitted");

                    b.Property<string[]>("Prefixes")
                        .HasColumnType("text[]")
                        .HasColumnName("prefixes");

                    b.HasKey("GuildId")
                        .HasName("pk_guild_settings");

                    b.ToTable("guild_settings");
                });

            modelBuilder.Entity("CheeseBot.EfCore.Entities.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Content")
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<ulong>("OwnerId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("owner_id");

                    b.HasKey("Id")
                        .HasName("pk_notes");

                    b.ToTable("notes");
                });

            modelBuilder.Entity("CheeseBot.EfCore.Entities.Reminder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<ulong>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<DateTime>("ExecutionTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("execution_time");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<ulong>("OriginalMessageId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("original_message_id");

                    b.Property<ulong>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_reminders");

                    b.ToTable("reminders");
                });

            modelBuilder.Entity("CheeseBot.EfCore.Entities.UserStopwatch", b =>
                {
                    b.Property<ulong>("Id")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("end_date");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_date");

                    b.HasKey("Id")
                        .HasName("pk_stopwatches");

                    b.ToTable("stopwatches");
                });
#pragma warning restore 612, 618
        }
    }
}