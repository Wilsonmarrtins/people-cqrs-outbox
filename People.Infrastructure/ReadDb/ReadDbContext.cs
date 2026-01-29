using Microsoft.EntityFrameworkCore;
using People.Infrastructure.ReadDb.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace People.Infrastructure.ReadDb;

public sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    public DbSet<PersonReadModel> People => Set<PersonReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersonReadModel>(e =>
        {
            e.ToTable("people_read");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Sex).HasMaxLength(20).IsRequired();
            e.Property(x => x.Rg).HasMaxLength(30).IsRequired();
            e.Property(x => x.Cpf).HasMaxLength(14).IsRequired();

            e.HasIndex(x => x.Rg).IsUnique();
            e.HasIndex(x => x.Cpf).IsUnique();
        });
    }
}
