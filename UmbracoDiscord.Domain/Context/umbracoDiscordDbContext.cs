using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using UmbracoDiscord.Domain.Entities;

namespace UmbracoDiscord.Domain.Context
{
    public partial class umbracoDiscordDbContext : DbContext
    {
        public umbracoDiscordDbContext()
        {
        }

        public umbracoDiscordDbContext(DbContextOptions<umbracoDiscordDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Stats> Stats { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("Data Source=../../../UmbracoDiscord.Domain/Database/umbracoDiscord.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stats>(entity =>
            {
                entity.Property(e => e.Experience)
                    .HasColumnType("NUMERIC")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ServerId).HasColumnType("STRING");

                entity.Property(e => e.UserId).HasColumnType("STRING");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
