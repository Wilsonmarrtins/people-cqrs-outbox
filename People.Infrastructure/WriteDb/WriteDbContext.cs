using Microsoft.EntityFrameworkCore;
using People.Domain.Entities;
using People.Infrastructure.WriteDb.Outbox;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace People.Infrastructure.WriteDb;

public sealed class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }

    public DbSet<Person> People => Set<Person>();
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(e =>
        {
            e.ToTable("People");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Sex).HasMaxLength(20).IsRequired();
            e.Property(x => x.Rg).HasMaxLength(30).IsRequired();
            e.Property(x => x.Cpf).HasMaxLength(14).IsRequired();

            e.HasIndex(x => x.Rg).IsUnique();
            e.HasIndex(x => x.Cpf).IsUnique();
        });

        modelBuilder.Entity<OutboxMessage>(e =>
        {
            e.ToTable("Outbox");
            e.HasKey(x => x.Id);

            e.Property(x => x.Type).HasMaxLength(200).IsRequired();
            e.Property(x => x.PayloadJson).IsRequired();
            e.Property(x => x.OccurredAtUtc).IsRequired();
            e.Property(x => x.ProcessedAtUtc);

            e.HasIndex(x => x.ProcessedAtUtc);
        });
    }
}
